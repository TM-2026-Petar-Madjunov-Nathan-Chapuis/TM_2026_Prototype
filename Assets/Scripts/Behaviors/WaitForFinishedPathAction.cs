using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitForFinishedPath", story: "[Agent] waits until he finished his nav mesh path", category: "Action/Delay", id: "6b1badf836aaeea283a4a21afa234399")]
public partial class WaitForFinishedPathAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(Agent.Value.GetComponent<NavMeshAgent>().pathPending) return Status.Running;
        if(Agent.Value.GetComponent<NavMeshAgent>().remainingDistance < 1f) return Status.Success; //if remaining distance smaller it makes some acceleration and decceleration issues
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

