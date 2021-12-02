using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Actor : Unit
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Unit unitTarget;
    [HideInInspector] public Coroutine currentTask;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AnimationEventListener animationEvent;
    [HideInInspector] public ActorVisualHandler visualHandler;
    override protected void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        animationEvent = GetComponentInChildren<AnimationEventListener>();
        visualHandler = GetComponent<ActorVisualHandler>();
        // animationEvent.attackEvent.AddListener(Attack);
    }
    override protected void Start()
    {
        base.Start();
    }
    public void Update()
    {
        animator.SetFloat("Speed", Mathf.Clamp(agent.velocity.magnitude, 0, 1));
    }

    public void SetDestination(Vector3 destination, float stoppingDistance = 2)
    {
        agent.destination = destination;
        agent.stoppingDistance = stoppingDistance;
    }
    public WaitUntil WaitForNavMesh()
    {
        return new WaitUntil(() => !agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance || (unitTarget && Vector3.Distance(unitTarget.transform.position, agent.pathEndPosition) >= 2f)));
    }
    void Attack()
    {
        if (unitTarget) {
            animator.SetTrigger("Attack");
            unitTarget.damageable.Hit(10);
        }
    }
    public void AttackTarget(Unit target)
    {
        unitTarget = target;

        currentTask = StartCoroutine(StartAttack());

        IEnumerator StartAttack()
        {
            while (unitTarget)
            {
                SetDestination(unitTarget.transform.position, unitTarget.radius);
                yield return WaitForNavMesh();
                while (unitTarget && Vector3.Distance(unitTarget.transform.position, transform.position) < unitTarget.radius + 1)
                {
                    transform.LookAt(unitTarget.transform);
                    yield return new WaitForSeconds(1);
                    Attack();
                }
            }
            StopTask();
        }
    }
    public virtual void SetTask(Collider collider)
    {
    }
    public virtual void StopTask()
    {
        if (currentTask != null)
            StopCoroutine(currentTask);
        unitTarget = null;
        currentTask = null;
    }
}
