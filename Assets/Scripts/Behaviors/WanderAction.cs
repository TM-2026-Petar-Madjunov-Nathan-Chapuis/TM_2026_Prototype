using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wander", story: "[Agent] wanders the world given a [radius] around [basePosition] at [Speed]", category: "Action/Navigation", id: "6aa8d9c96cd5d13a357d304324abfbea")]
public partial class WanderAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<Vector3> BasePosition;

    protected override Status OnStart()
    {
        float angle = UnityEngine.Random.Range(0, 360);
        float distance = UnityEngine.Random.value;

        Vector3 direction = new Vector3(math.cos(angle), 0, math.sin(angle));
        Vector3 position = BasePosition + direction * Radius * distance;

        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            Agent.Value.GetComponent<NavMeshAgent>().SetDestination(hit.position);
            Agent.Value.GetComponent<NavMeshAgent>().speed = Speed;
            return Status.Running;
        }
        if (Agent.Value.GetComponent<NavMeshAgent>().remainingDistance < 5f) return this.OnStart(); //reroll if the position is too close.
        return Status.Success;
    }
}

