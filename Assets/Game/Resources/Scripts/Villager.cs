using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Actor
{
    [HideInInspector] public Building currentBuilding;
    [HideInInspector] public Resource currentResource;
    private ResourcesType resourceType;
    private int resourceAmount;

    override protected void Start()
    {
        base.Start();
        currentBuilding = null;
        currentResource = null;
    }
    private void DropResource()
    {
        BuildingManager.instance.AddResource(resourceType, resourceAmount);
        resourceAmount = 0;
    }
    public void DropResources(Building building)
    {
        currentBuilding = building;

        if (currentTask != null)
            StopCoroutine(currentTask);

        currentTask = StartCoroutine(GoAndDrop());

        IEnumerator GoAndDrop()
        {
            SetDestination(currentBuilding.transform.position, currentBuilding.radius);
            yield return WaitForNavMesh();
            if (building.TryGetComponent(out DropOffResources dropOff) && dropOff.CanDrop(resourceType))
            {
                transform.LookAt(currentBuilding.transform);
                DropResource();
            }
            StopTask();
        }
    }
    private void DoWork()
    {
        if (currentBuilding) {
            animator.SetTrigger("Attack");
            currentBuilding.Build(10);
        }
    }
    public void GiveJob(Building job)
    {
        currentBuilding = job;

        if (currentTask != null)
            StopCoroutine(currentTask);

        currentTask = StartCoroutine(StartJob());
        
        IEnumerator StartJob()
        {
            Vector3 jobPosition = job.transform.position;
            Vector2 randomPosition = Random.insideUnitCircle.normalized * currentBuilding.radius;
            jobPosition.x += randomPosition.x;
            jobPosition.z += randomPosition.y;
            SetDestination(jobPosition);
            yield return WaitForNavMesh();
            while (!currentBuilding.IsFinished())
            {
                transform.LookAt(currentBuilding.transform);
                yield return new WaitForSeconds(1);
                DoWork();
            }
            StopTask();
        }
    }
    private void Mine()
    {
        if (currentResource) {
            animator.SetTrigger("Attack");
            currentResource.HitResource(10, this);
        }
    }
    public void MineResource(Resource resource)
    {
        currentResource = resource;
        unitTarget = resource.unit;

        if (currentTask != null)
            StopCoroutine(currentTask);

        currentTask = StartCoroutine(StartMining());

        IEnumerator StartMining()
        {
            SetDestination(currentResource.transform.position, currentResource.unit.radius);
            yield return WaitForNavMesh();
            while (currentResource && Vector3.Distance(currentResource.transform.position, transform.position) < currentResource.unit.radius + 1)
            {
                transform.LookAt(currentResource.transform);
                yield return new WaitForSeconds(1);
                Mine();
            }
            StopTask();
        }
    }
    public override void SetTask(Collider collider)
    {
        if (collider.TryGetComponent(out Building building))
        {
            if (!building.IsFinished())
            {
                StopTask();
                GiveJob(building);
            } else if (building.TryGetComponent(out DropOffResources dropOff))
            {
                StopTask();
                DropResources(building);
            }
        }
        else if (collider.TryGetComponent(out Resource resource))
        {
            StopTask();
            MineResource(resource);
        }
        else if (!collider.CompareTag("Player"))
        {
            if (collider.TryGetComponent(out Unit unit))
            {
                StopTask();
                AttackTarget(unit);
            }
        }
    }
    override public void StopTask()
    {
        base.StopTask();
        currentBuilding = null;
        currentResource = null;
    }
    public void IncreaseResource(ResourcesType type, int amount)
    {
        if (resourceType != type) {
            resourceAmount = 0;
            resourceType = type;
        }
        resourceAmount += amount;
    }
}
