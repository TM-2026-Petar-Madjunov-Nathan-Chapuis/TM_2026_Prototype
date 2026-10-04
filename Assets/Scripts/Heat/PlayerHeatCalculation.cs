using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TM.Player;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHeatCalculation : MonoBehaviour
{

    [SerializeField] private float baseSourceTemperature = 100f;
    [SerializeField] private float baseSourcePower = 1f;

    private float ambiantTemperature;
    private List<GameObject> heatObjects;
    private Dictionary<GameObject, float> heatObjectsPlayersHeat = new Dictionary<GameObject, float>(); // --> object, heat that provide at player position



    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerCordonates;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private WorldInfos worldInfos;

    void Awake()
    {
        heatObjects = worldInfos.environnementObjects;
        ambiantTemperature = worldInfos.ambientTemperature;
    }
    void Update()
    {   
        calculateNewPlayersAmbiantTemperature();
    }

    void GetHeatForEachHeatObject()
    {
        float distancePlayerSource;
        
        foreach (GameObject obj in worldInfos.environnementObjects)
        {
            {
                if(obj.TryGetComponent<HeatObject>(out _))
                {
                    distancePlayerSource = Vector3.Distance(obj.transform.position, playerCordonates.position);
                    
                    float newPotentialTemperature = (ambiantTemperature + (baseSourceTemperature-ambiantTemperature)) * (baseSourcePower/(1+math.max(0, distancePlayerSource*distancePlayerSource))); // normallement distancePlayerSource n'est pas ^2
                    heatObjectsPlayersHeat[obj] = newPotentialTemperature; // obj, temperature at player position
                }
            } 
        }
    }
    GameObject FindHeatestHeatObject()
    {
        GameObject HeatestObject = null;

        foreach (GameObject obj in heatObjectsPlayersHeat.Keys)
        {
            if(HeatestObject == null || heatObjectsPlayersHeat[HeatestObject] < heatObjectsPlayersHeat[obj])
            {
                HeatestObject = obj;
            }
        }
        return HeatestObject;
    }

    void calculateNewPlayersAmbiantTemperature()
    {
        GetHeatForEachHeatObject();

        if (heatObjectsPlayersHeat.Count == 0)
        {
            playerInfo.playerFeelAmbiantTemperature = ambiantTemperature;
            //Debug.Log($"0{ambiantTemperature}");
        }
        else
        {
            playerInfo.playerFeelAmbiantTemperature = heatObjectsPlayersHeat[FindHeatestHeatObject()];
        }
    }
}
