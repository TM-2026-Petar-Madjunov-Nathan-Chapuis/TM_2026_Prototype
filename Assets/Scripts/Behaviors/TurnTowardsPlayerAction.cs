using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TurnTowardsPlayer", story: "[Agent] Turns Towards [Position]", category: "Action/Navigation", id: "e6b2b2c8b7dc890a96bc3e2685ce9301")]
public partial class TurnTowardsPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> Position;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Position.Value == null) return Status.Failure;
        Agent.Value.GetComponent<NavMeshAgent>().updateRotation = false;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector3 direction = Position.Value - Agent.Value.transform.position;
        direction.y = 0;//ignore the height difference. for now.

        Quaternion targetRot = Quaternion.LookRotation(direction);
        Agent.Value.transform.rotation = Quaternion.RotateTowards(Agent.Value.transform.rotation, targetRot, 120f * Time.deltaTime); //smoothly rotate the agent at 120degress second.
        
        if (Quaternion.Angle(Agent.Value.transform.rotation, targetRot) < 1f) return Status.Success; // close enough that its success
        return Status.Running;
    }

    protected override void OnEnd()
    {
        Agent.Value.GetComponent<NavMeshAgent>().updateRotation = true;
    }
}

