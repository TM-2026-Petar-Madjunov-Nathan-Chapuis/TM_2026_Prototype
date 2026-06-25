using System;
using UnityEngine;

public class HealthCalculation : MonoBehaviour
{   
    [SerializeField] private float timeCoefficient = 0.05f;
    [SerializeField] private float HealthBecauseHungryCoefficient = 1f; //coeff de vitesse de perte de vie quand faim
    [SerializeField] private float HealthBecauseThirstCoefficient = 1f; //coeff de vitesse de perte de vie quand soif
    [SerializeField] private float healthBecauseHypothermiaCoefficient = 1f; //coeff de vitesse de perte de vie quand hypothermie
    [SerializeField] private float healthBecauseHyperthermiaCoefficient = 1f; //coeff de vitesse de perte de vie quand hyperthermie
    [SerializeField] private float maximalHypothermicAndHyperthermicCoefficient = 10f;
    [SerializeField] private HungerAndThirstCalculation hungerAndThirstCalculation;
    [SerializeField] private GameObject player;
    private PlayerInfo playerInfo;

    void Awake()
    {
        playerInfo = player.GetComponent<PlayerInfo>();
    }
    void Start()
    {

    }
    
    void LooseHealthbecauseHungryOrThirst()
    {
        if(playerInfo.playerHunger <= 0)
        {
            HealthBecauseHungryCoefficient = hungerAndThirstCalculation.hungerMetabolismCoefficient;
            HealthBecauseHungryCoefficient = Math.Clamp(HealthBecauseHungryCoefficient, 1, 11); // par securité
            HealthBecauseHungryCoefficient = ((HealthBecauseHungryCoefficient-1) /10)+1;

            playerInfo.playerHealth -= timeCoefficient*HealthBecauseHungryCoefficient*Time.deltaTime;
        }
        if(playerInfo.playerThirst <= 0)
        {
            HealthBecauseThirstCoefficient = hungerAndThirstCalculation.thirstMetabolismCoefficient;
            HealthBecauseThirstCoefficient = Math.Clamp(HealthBecauseThirstCoefficient, 1, 11); // par securité
            HealthBecauseThirstCoefficient = ((HealthBecauseThirstCoefficient-1) /10)+1;

            playerInfo.playerHealth -= timeCoefficient*HealthBecauseThirstCoefficient*Time.deltaTime;
        }

    }

    void LooseHealthbecauseCorporalTempearture()
    {
        void Hypothermia()
        {
            if (playerInfo.playerCorporalTemperature <= playerInfo.hypothermiaTemperature)
            {
                healthBecauseHypothermiaCoefficient = ((playerInfo.hypothermiaTemperature - playerInfo.playerCorporalTemperature)/(playerInfo.hypothermiaTemperature - playerInfo.coldTemperatureLimit))*maximalHypothermicAndHyperthermicCoefficient;
                
                playerInfo.playerHealth -= timeCoefficient*healthBecauseHypothermiaCoefficient*Time.deltaTime;
            }

            if (playerInfo.playerCorporalTemperature <= playerInfo.coldTemperatureLimit)
            {
                playerInfo.playerHealth = 0f;
            }

        }
        void Hyperthermia()
        {
            if (playerInfo.playerCorporalTemperature >= playerInfo.hyperthermiaTemperature)
            {
                healthBecauseHyperthermiaCoefficient = ((playerInfo.playerCorporalTemperature - playerInfo.hyperthermiaTemperature)/(playerInfo.hotTemperatureLimit - playerInfo.hyperthermiaTemperature))*maximalHypothermicAndHyperthermicCoefficient;
                
                playerInfo.playerHealth -= timeCoefficient*healthBecauseHyperthermiaCoefficient*Time.deltaTime;
            }

            if (playerInfo.playerCorporalTemperature >= playerInfo.hotTemperatureLimit)
            {
                playerInfo.playerHealth = 0f;
            }
        }

        Hypothermia();
        Hyperthermia();
    }

    void Update()
    {
        LooseHealthbecauseHungryOrThirst();
        LooseHealthbecauseCorporalTempearture();
        playerInfo.playerHealth = Math.Clamp(playerInfo.playerHealth, 0, 100);
    }
}
