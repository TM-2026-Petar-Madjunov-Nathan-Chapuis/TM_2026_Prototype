using System;
using Newtonsoft.Json;
using TM.Saving;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
namespace TM.Player
{
    public class PlayerInfo : HealthManager, ISaveable
    {

        public float playerHunger; // 100 -> 0
        public float playerThirst; // 100 -> 0


        public float playerNormalCorporalTemperature = 37f; //Constante 
        public float coldTemperatureLimit = 24f; //Constante // la vie est perdue 10* plus vite et mort
        public float hotTemperatureLimit = 43f; //Constante // la vie est perdue 10* plus vite et mort
        public float hypothermiaTemperature = 35f; //Constante // la vie est perdue 1* plus vite (=)
        public float hyperthermiaTemperature = 39f; //Constante // la vie est perdue 1* plus vite (=)

        public float playerCorporalTemperature; // [24; 43] < 28 mort, > 42 mort
        public float playerThermalBlilan;
        public float playerClothesResistance = 0f; // 0 -> 1

        public float playerMetabolismWork; // -20 <-> 20 // (sans playerBaseMetabolismwork)
        public float playerBaseMaximumMetabolismwork = 20f; //Constante //ce que le metabolisme peut thermoréguler dans les meilleurs conditions
        public float playerMinimumMetabolismWork = -20f;
        public float playerMaximumMetabolismWork = 20f;
        public float playerBaseMetabolismwork = 8f; //Constante //le metabolisme produit par le corp quoi qu'il arrive

        public string UID => "PlayerInfos";

        void Start()
        {
            this.health = 100;
        }

        public override void TakeDamage(float ammount, string source)
        {
            Debug.Log("Player took damage : " + ammount + " by " + source);
            this.health -= ammount;
            if (this.health < 0)
            {
                this.Die(source);
            }
        }

        public override void RestoreHealth(float ammount)
        {
            Debug.Log("player restored health : " + ammount);
            this.health += ammount;
            Mathf.Clamp(0, 100, health);
        }

        protected override void Die(string source)
        {
            Debug.Log("player died by " + source); 
            this.health = 100;//for now just restore health
        }

        public object SaveData()
        {
            PlayerSaveData playerSaveData = new PlayerSaveData
            {
                playerHunger = this.playerHunger,
                playerThirst = this.playerThirst,
                playerCorporalTemperature = this.playerCorporalTemperature,
                playerMetabolismWork = this.playerMetabolismWork,
                playerHealth = this.health,
            };
            return playerSaveData;
        }

        public void LoadData(string data)
        {
            PlayerSaveData saveData = JsonConvert.DeserializeObject<PlayerSaveData>(data);
            this.playerHunger = saveData.playerHunger;
            this.playerThirst = saveData.playerThirst;
            this.playerMetabolismWork = saveData.playerMetabolismWork;
            this.playerCorporalTemperature = saveData.playerCorporalTemperature;
            this.health = saveData.playerHealth;
        }
    }
    [Serializable]
    public struct PlayerSaveData
    {
        public float playerHunger;
        public float playerThirst;
        public float playerCorporalTemperature;
        public float playerMetabolismWork;
        public float playerHealth;
    }
}
