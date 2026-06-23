using System;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;



public class CorporalTemperatureCalculation : MonoBehaviour
{   
    //[SerializeField] private float playerNormalCorporalTemperature = 36.5f;
    //[SerializeField] private float clothesResistance = 1f; // 0 -> 1
    [SerializeField] float ambientTemperature;

    //[field: SerializeField]public float playerCorporalTemperature{get; private set;}
    //[SerializeField] private float playerMetabolismWork; // in °
    //[SerializeField] private float playerThermalBlilan; // in °
    [SerializeField] private float deltaTemperature;
    [SerializeField] private float timeCoefficient = 0.005f;
    private float wherePlayerHungerAndThirstIsTooLowToMaintainMetabolism = 50f; //à partir de combien, le metabolisme ne régule pas de 20°
    private float minimalBaseMetabloismWork = 10f; // le régulation minimale de temperature même si le joueur a 0 eau et 0 nourriture
    [SerializeField] private GameObject player;
    private PlayerInfo playerInfo;


    void Awake()
    {   
        playerInfo = player.GetComponent<PlayerInfo>();

        WorldInfos worldInfos = new WorldInfos();
        ambientTemperature = worldInfos.ambientTemperature;
    }

    void Start()
    {
       playerInfo.playerCorporalTemperature = playerInfo.playerNormalCorporalTemperature;
    }

    void CalculatePlayerCorporalTemperature()
    {
        deltaTemperature = playerInfo.playerCorporalTemperature-ambientTemperature; //ce que l'air vole au joueur

        playerInfo.playerMetabolismWork = playerInfo.playerNormalCorporalTemperature -playerInfo.playerCorporalTemperature + playerInfo.playerClothesResistance*(deltaTemperature-playerInfo.playerBaseMetabolismwork);
        if(playerInfo.playerHunger <= wherePlayerHungerAndThirstIsTooLowToMaintainMetabolism) // =50
        {
            
            playerInfo.playerMaximumMetabolismWork = ((playerInfo.playerMaximumMetabolismWork/2)*playerInfo.playerHunger/wherePlayerHungerAndThirstIsTooLowToMaintainMetabolism) + minimalBaseMetabloismWork;
        }
        else
        {
            playerInfo.playerMaximumMetabolismWork = playerInfo.playerBaseMaximumMetabolismwork;
        }
        if(playerInfo.playerThirst <= wherePlayerHungerAndThirstIsTooLowToMaintainMetabolism) // =50
        {
            playerInfo.playerMinimumMetabolismWork = ((playerInfo.playerMinimumMetabolismWork/2)*playerInfo.playerThirst/wherePlayerHungerAndThirstIsTooLowToMaintainMetabolism) - minimalBaseMetabloismWork;
        }
        else
        {
            playerInfo.playerMinimumMetabolismWork = -playerInfo.playerBaseMaximumMetabolismwork;
        }

        playerInfo.playerMetabolismWork = Math.Clamp(playerInfo.playerMetabolismWork, playerInfo.playerMinimumMetabolismWork, playerInfo.playerMaximumMetabolismWork);
        
        playerInfo.playerThermalBlilan = -(playerInfo.playerClothesResistance * deltaTemperature) + (playerInfo.playerMetabolismWork+playerInfo.playerBaseMetabolismwork);
        playerInfo.playerCorporalTemperature += playerInfo.playerThermalBlilan*timeCoefficient*Time.deltaTime;
    }

    void Hypothermia()
    {
        if (playerInfo.playerCorporalTemperature <= playerInfo.playerNormalCorporalTemperature-2)
        {
            
        }


    }

    void Update()
    {
        CalculatePlayerCorporalTemperature();
    }
}
