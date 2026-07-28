using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AgentApprochesTarget", story: "[Agent] approaches [Target] by [StepSize] at [CreepingSpeed]", category: "Action/Navigation", id: "a55554e3fab536295c71601d35bfdde9")]
public partial class AgentApprochesTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> StepSize;
    [SerializeReference] public BlackboardVariable<float> CreepingSpeed;
    private NavMeshAgent agent;

    protected override Status OnStart()
    {

        if (Target.Value == null)
        {
            Debug.LogError("Target is null!");
            return Status.Failure;
        }
        agent = Agent.Value.GetComponent<NavMeshAgent>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        NavMeshPath path = new();
        NavMeshHit hit;
        if (NavMesh.SamplePosition(Target.Value.transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            agent.CalculatePath(hit.position, path);
        }
        Vector3 targetPosition = (path.corners[1] - Agent.Value.transform.position).normalized * StepSize.Value + Agent.Value.transform.position;
        agent.speed = CreepingSpeed;
        Agent.Value.GetComponent<Animator>().SetBool("Creeping", true);
        agent.SetDestination(targetPosition);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

