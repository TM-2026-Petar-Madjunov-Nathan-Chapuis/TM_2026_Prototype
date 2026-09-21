using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine.UIElements;
using UnityEngine.AI;
using UnityEditor.Analytics;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Flee", story: "[Agent] flees from [target] at [distance] at [fleeSpeed]", category: "Action", id: "e52c63e0ee54dd4aa62b10cb87c11156")]
public partial class FleeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Distance;
    [SerializeReference] public BlackboardVariable<float> FleeSpeed;
    private NavMeshAgent navMeshAgent;

    protected override Status OnStart()
    {

        if (Target.Value == null)
        {
            Debug.LogError("Target is null!");
            return Status.Failure;
        }
        navMeshAgent = Agent.Value.GetComponent<NavMeshAgent>();
        Vector3 awayDirection = (Agent.Value.transform.position - Target.Value.transform.position).normalized;
        awayDirection.y = 0;
        float angle = UnityEngine.Random.Range(-30f, 30f);

        awayDirection = Quaternion.Euler(0, angle, 0) * awayDirection; //steer the direction by the angle. credits for this line ai
        Vector3 fleeTarget = Agent.Value.transform.position + awayDirection * Distance.Value;
        Debug.DrawLine(Agent.Value.transform.position, fleeTarget, Color.red);
        
        if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, Distance, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
            navMeshAgent.speed = FleeSpeed;
            return Status.Running;
        }
        Debug.Log("the navmeshagent raycast failed");
        return Status.Failure;
    }

    protected override Status OnUpdate()
    {
        if (navMeshAgent.pathPending) return Status.Running;

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) return Status.Success; //wait for the end of the path

        return Status.Running;
    }

    protected override void OnEnd()
    {
        
    }
}

