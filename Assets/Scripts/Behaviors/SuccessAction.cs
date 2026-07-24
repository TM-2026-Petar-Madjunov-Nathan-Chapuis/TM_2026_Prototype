using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Success", story: "Success", category: "Action", id: "46baf9f0d799e0fb96739713c7adc4bf")]
public partial class SuccessAction : Action
{
    protected override Status OnStart()
    {
        return Status.Success;
    }
}

