using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;
    List<Building> allBuildings = new List<Building>();
    [SerializeField] public SerializedDictionary<ResourcesType, int> currentResources;
    [SerializeField] private ParticleSystem buildParticle;
    [SerializeField] private ParticleSystem finishParticle;
    [Header("UI display")]
    [SerializeField] public Transform resourceGroup;
    [Header("BuildingPlacement")]
    Building currentBuilding = null;
    Mesh buildingPreviewMesh;
    BoxCollider buildingPreviewCollider;
    [SerializeField] Material buildingPreviewMat;
    [SerializeField] LayerMask mask;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        RefreshResources();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Collider collider = Utility.CameraRay().collider;
            foreach (Building building in allBuildings)
            {
                if (building.isHover)
                    UIManager.instance.SetSelectedUnitMenu(building);
                break;
            }
        }
        if (currentBuilding)
        {
            Vector3 position = Utility.MouseToTerrainPosition();

            bool isColliding = Physics.CheckBox(position, buildingPreviewCollider.size / 2, buildingPreviewCollider.transform.rotation, mask.value);
            if (isColliding) {
                buildingPreviewMat.color = Color.red;
            } else {
                buildingPreviewMat.color = Color.blue;
            }
            Graphics.DrawMesh(buildingPreviewMesh, position, Quaternion.identity, buildingPreviewMat, 0);
            if (Input.GetMouseButtonDown(1) && !isColliding)
            {
                BuildingManager.instance.SpawnBuilding(currentBuilding, position);
                currentBuilding = null;
            }
            if (Input.GetMouseButtonDown(0))
            {
                currentBuilding = null;
            }
        }
    }
    
    public void SpawnBuilding(Building currentBuilding, Vector3 position)
    {
        if (!currentBuilding.CanCreate(currentResources) || ActorManager.instance.selectedActors.Count <= 0) {
            currentBuilding = null;
            return;
        }

        // Create Building
        Building building = Instantiate(currentBuilding, position, Quaternion.identity);
        allBuildings.Add(building);
        building.damageable.onDestroy.AddListener(() => RemoveBuilding(building));

        // Subtract resources
        Dictionary<ResourcesType, int> cost = building.Cost();
        foreach (var resourceCost in cost)
        {
            currentResources[resourceCost.Key] -= resourceCost.Value;
            RefreshResources();
        }
    }

    public List<Building> GetBuildings()
    {
        return allBuildings;
    }
    public void RemoveBuilding(Building building)
    {
        allBuildings.Remove(building);
    }
    public void AddResource(ResourcesType resourceType, int amount)
    {
        currentResources[resourceType] += amount;

        RefreshResources();
    }
    public void PlayParticle(Vector3 position)
    {
        if (buildParticle)
        {
            buildParticle.transform.position = position;
            buildParticle.Play();
        }
    }
    public void PlayFinishParticle(Vector3 position)
    {
        if (finishParticle)
        {
            finishParticle.transform.position = position;
            finishParticle.Play();
        }
    }
    public void SelectBuilding(Building building)
    {
        currentBuilding = building;
        buildingPreviewMesh = building.GetComponentInChildren<MeshFilter>().sharedMesh;
        buildingPreviewCollider = building.GetComponent<BoxCollider>();
    }
    public void RefreshResources()
    {
        for (int i = 0; i < resourceGroup.childCount; i++)
            resourceGroup.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = BuildingManager.instance.currentResources[(ResourcesType)i].ToString();
    }
}
