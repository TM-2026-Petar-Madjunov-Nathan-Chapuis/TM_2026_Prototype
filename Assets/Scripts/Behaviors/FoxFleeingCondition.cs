using System;
using Unity.Behavior;
using Unity.Mathematics;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "FoxFleeing", story: "[Agent] doesnt flees from [Player] based on [distance]", category: "Conditions", id: "ef88240dc9f6dddd296c00ad63b01999")]
public partial class FoxFleeingCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<float> Distance;

    public override bool IsTrue()
    {
        float distance = math.clamp(Distance.Value, 2f, 20f);
        float fleeChance = Mathf.InverseLerp(20f, 2f, distance); // 0% at 20, 100% at 2 meters.
        if (UnityEngine.Random.value < fleeChance) return false;
        return true;
    }
}
