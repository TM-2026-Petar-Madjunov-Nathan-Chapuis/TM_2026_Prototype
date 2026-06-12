using System;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;



public class ColdMechanic : MonoBehaviour
{   
    [SerializeField] private float normalPlayerCorporalTemperature = 36.5f;
    [SerializeField] private float clothesResistance = 1f; // 0 -> 1
    [SerializeField] float ambientTemperature;
    public float playerCorporalTemperature{get; private set;}
    [SerializeField] private float metabolismWork; // in °
    private float deltaTemperature;
    [SerializeField] private float deltaTime = 1f;


    void Start()
    {
        WorldInfos worldInfos = new WorldInfos();
        ambientTemperature = worldInfos.ambientTemperature;

        PlayerInfo playerInfo = new PlayerInfo();
        playerCorporalTemperature = playerInfo.playerCorporalTemperature;

        playerCorporalTemperature = normalPlayerCorporalTemperature;
    }

    void CalculatePlayerCorporalTemperature()
    {
        deltaTemperature = playerCorporalTemperature-ambientTemperature;

        metabolismWork = Math.Clamp(metabolismWork, -20f, 20f);
        metabolismWork = -(clothesResistance * deltaTemperature) + (normalPlayerCorporalTemperature - playerCorporalTemperature);

        playerCorporalTemperature = playerCorporalTemperature - (clothesResistance*deltaTemperature+metabolismWork)*deltaTime;
    }

    void Hypothermia()
    {
        if (playerCorporalTemperature <= normalPlayerCorporalTemperature-2)
        {
            
        }


    }
 
    // Update is called once per frame
    void Update()
    {
        CalculatePlayerCorporalTemperature();
    }
}
