using TM.Player;
using Unity.Behavior;
using UnityEngine;

/* 
   variables : 
   CanSeePlayer - boolean
   PlayerDistance - float
   LastSeenPosition - vector3
   Player - gamemobject
   HeadPlayer - boolean
   FleeDistance - float
   WanderRadius - float 
   LastHeardPosition - vector 3 
   PlayerIsMoving - boolean
*/

public class FoxSensor : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float maxSeeingDistance;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float FleeDistance;
    [SerializeField] private float WanderRadius;
    private BehaviorGraphAgent behaviorGraphAgent;
    private float timer;

    void Start()
    {
        this.behaviorGraphAgent = this.GetComponent<BehaviorGraphAgent>();
        this.behaviorGraphAgent.SetVariableValue("WanderRadius", WanderRadius);
        this.behaviorGraphAgent.SetVariableValue("FleeDistance", FleeDistance);
        this.behaviorGraphAgent.SetVariableValue("Player",player);
        this.behaviorGraphAgent.SetVariableValue("basePosition", this.transform.position);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer < 0.2f) 
            return;
        UpdateSensors();
        timer = 0;   
    }
    private void UpdateSensors()
    {
        float distance = Vector3.Distance(player.transform.position, this.transform.position);
        this.behaviorGraphAgent.SetVariableValue("CanSeePlayer", CheckIfPlayerIsInSight(distance));
        this.behaviorGraphAgent.SetVariableValue("PlayerDistance", distance);
        this.behaviorGraphAgent.SetVariableValue("PlayerIsMoving", (bool)(player.GetComponent<CharacterController>().velocity.magnitude > 0.01f));
    }

    public bool CheckIfPlayerIsInSight(float dis)
    {
        if (dis > maxSeeingDistance) return false; //player too far
        Ray ray = new Ray(this.transform.position, player.transform.position - this.transform.position);
        Debug.DrawLine(this.transform.position, player.transform.position, Color.azure);
        if (Physics.Raycast(ray, out RaycastHit hit, dis, layerMask))
        {
            return false; //something hit, meaning the player isnt in sight
        }
        return true;
    }
}
