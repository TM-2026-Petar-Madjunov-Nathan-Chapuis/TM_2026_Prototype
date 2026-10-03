using System;
using UnityEngine;

namespace TM.Player
{
    public class HealthCalculation : MonoBehaviour
    {   
        [field: SerializeField] public float roundedHealth { get; private set; }
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
            if (playerInfo.playerHunger <= 0)
            {
                HealthBecauseHungryCoefficient = hungerAndThirstCalculation.hungerMetabolismCoefficient;
                HealthBecauseHungryCoefficient = Math.Clamp(HealthBecauseHungryCoefficient, 1, 11); // par securité
                HealthBecauseHungryCoefficient = ((HealthBecauseHungryCoefficient - 1) / 10) + 1;

                playerInfo.TakeDamage(timeCoefficient * HealthBecauseHungryCoefficient * Time.deltaTime, "Hunger");
            }
            if (playerInfo.playerThirst <= 0)
            {
                HealthBecauseThirstCoefficient = hungerAndThirstCalculation.thirstMetabolismCoefficient;
                HealthBecauseThirstCoefficient = Math.Clamp(HealthBecauseThirstCoefficient, 1, 11); // par securité
                HealthBecauseThirstCoefficient = ((HealthBecauseThirstCoefficient - 1) / 10) + 1;

                playerInfo.TakeDamage(timeCoefficient * HealthBecauseThirstCoefficient * Time.deltaTime, "Thirst");
            }

        }

        void LooseHealthbecauseCorporalTempearture()
        {
            void Hypothermia()
            {
                if (playerInfo.playerCorporalTemperature <= playerInfo.hypothermiaTemperature)
                {
                    healthBecauseHypothermiaCoefficient = ((playerInfo.hypothermiaTemperature - playerInfo.playerCorporalTemperature) / (playerInfo.hypothermiaTemperature - playerInfo.coldTemperatureLimit)) * maximalHypothermicAndHyperthermicCoefficient;

                    playerInfo.TakeDamage(timeCoefficient * healthBecauseHypothermiaCoefficient * Time.deltaTime, "Hypothermia");
                }

                if (playerInfo.playerCorporalTemperature <= playerInfo.coldTemperatureLimit)
                {
                    playerInfo.TakeDamage(10000f, "Hyperthermia"); //essentially kill
                }

            }
            void Hyperthermia()
            {
                if (playerInfo.playerCorporalTemperature >= playerInfo.hyperthermiaTemperature)
                {
                    healthBecauseHyperthermiaCoefficient = ((playerInfo.playerCorporalTemperature - playerInfo.hyperthermiaTemperature) / (playerInfo.hotTemperatureLimit - playerInfo.hyperthermiaTemperature)) * maximalHypothermicAndHyperthermicCoefficient;

                    playerInfo.TakeDamage(timeCoefficient * healthBecauseHyperthermiaCoefficient * Time.deltaTime, "Hyperthermia");
                }

                if (playerInfo.playerCorporalTemperature >= playerInfo.hotTemperatureLimit)
                {
                    playerInfo.TakeDamage(10000f, "Hyperthermia"); //essentially kill
                }
            }

            Hypothermia();
            Hyperthermia();
        }

        void Update()
        {
            LooseHealthbecauseHungryOrThirst();
            LooseHealthbecauseCorporalTempearture();
            roundedHealth = (float)Math.Round(playerInfo.health);
        }
    }
}