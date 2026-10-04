using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class FoxController : MonoBehaviour
{
    [SerializeField] private float fleeSpeed; //flee speed is 2 of the blend tree
    [SerializeField] private float normalSpeed; //normal speed is 1 of the blend tree
    [SerializeField] private float creepingSpeed; 
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private static readonly int SpeedMagnitudeHash = Animator.StringToHash("SpeedMagnitude"); // better performance according to unity UNT0041 warning (store hash reference to parameter rather than repeated id lookup)

    void Start()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        this.GetComponent<BehaviorGraphAgent>().SetVariableValue("fleeSpeed", fleeSpeed);
        this.GetComponent<BehaviorGraphAgent>().SetVariableValue("normalSpeed", normalSpeed);
        this.GetComponent<BehaviorGraphAgent>().SetVariableValue("creepingSpeed", creepingSpeed);
    }
    void Update()
    {
        float magnitude = Math.Clamp(navMeshAgent.velocity.magnitude, 0, fleeSpeed); 
        if (magnitude <= normalSpeed) magnitude = magnitude / normalSpeed; //gives 0 - 1 when value under normal speed
        else if (magnitude > normalSpeed) magnitude = (magnitude - normalSpeed) / (fleeSpeed - normalSpeed) + 1; // gives 1 - 2 when value above normal speed¨
        if (navMeshAgent.speed == creepingSpeed) magnitude = 1;
        if (magnitude < 0.25f)
        {
            magnitude = 0;
        }
        animator.SetFloat(SpeedMagnitudeHash, magnitude);
    }
}
