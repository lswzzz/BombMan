using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class MovementTask : Action
{
    public override void OnStart()
    {

    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}