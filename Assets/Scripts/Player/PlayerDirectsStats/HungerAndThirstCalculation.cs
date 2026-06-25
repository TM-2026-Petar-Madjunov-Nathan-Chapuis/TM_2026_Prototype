using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class HungerAndThirstCalculation : MonoBehaviour
{   
    //[SerializeField] private int roundedPlayerHunger;
    //[SerializeField] private int roundedplayerThirst;
    [field: SerializeField] public float hungerMetabolismCoefficient{get; private set; }
    [field: SerializeField] public float thirstMetabolismCoefficient{get; private set; }
    [SerializeField] private float timeCoefficient = 0.005f;
    [SerializeField] private GameObject player;
    private PlayerInfo playerInfo;
    

    void Awake()
    {
        playerInfo = player.GetComponent<PlayerInfo>();
    }
    void Start()
    {

    }
    
    float CoefficientsCalculation(string whichCoefficient)
    {
        if (playerInfo.playerMetabolismWork >= 0)
        {
            hungerMetabolismCoefficient = playerInfo.playerMetabolismWork + 1; // 1 -> 21
            thirstMetabolismCoefficient = 1;
        }

        if (playerInfo.playerMetabolismWork <= 0)
        {
            thirstMetabolismCoefficient = -playerInfo.playerMetabolismWork + 1; // 1 -> 21
            hungerMetabolismCoefficient = 1;
        }


        if (whichCoefficient == "hungerMetabolismCoefficient")
        {
            return hungerMetabolismCoefficient;
        }
        else if (whichCoefficient == "thirstMetabolismCoefficient")
        {
            return thirstMetabolismCoefficient;
        }
        else
        {
            Debug.Log("Le parametre selectioné n'existe pas comme coeff, valeur par défault : 1");
            return 1;
            
        }
    }
    void HungerCalculation()
    {
        hungerMetabolismCoefficient = CoefficientsCalculation("hungerMetabolismCoefficient");

        playerInfo.playerHunger -= timeCoefficient*hungerMetabolismCoefficient*Time.deltaTime;
        playerInfo.playerHunger = Math.Clamp(playerInfo.playerHunger, 0f, 100f);
        //roundedPlayerHunger = (int)Math.Round(playerInfo.playerHunger);
    }
    
    void ThirstCalculation()
    {
        thirstMetabolismCoefficient = CoefficientsCalculation("thirstMetabolismCoefficient");

        playerInfo.playerThirst -= timeCoefficient*thirstMetabolismCoefficient*Time.deltaTime;
        playerInfo.playerThirst = Math.Clamp(playerInfo.playerThirst, 0f, 100f);
        //roundedplayerThirst = (int)Math.Round(playerInfo.playerThirst);
    }
    void Update()
    {
        HungerCalculation();
        ThirstCalculation();
    }
}
