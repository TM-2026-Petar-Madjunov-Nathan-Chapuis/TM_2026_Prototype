using UnityEngine;
namespace TM.Player
{
    public float playerHealth; // 100 -> 0


    public float playerHunger; // 100 -> 0
    public float playerThirst; // 100 -> 0


    public float playerNormalCorporalTemperature = 37f; //Constante 
    public float coldTemperatureLimit = 24f; //Constante // la vie est perdue 10* plus vite et mort
    public float hotTemperatureLimit = 43f; //Constante // la vie est perdue 10* plus vite et mort
    public float hypothermiaTemperature = 35f; //Constante // la vie est perdue 1* plus vite (=)
    public float hyperthermiaTemperature = 39f; //Constante // la vie est perdue 1* plus vite (=)

    public float playerCorporalTemperature; // [25; 45] < 28 mort, > 42 mort
    public float playerThermalBlilan;
    public float playerClothesResistance = 0f; // 0 -> 1

    public float playerMetabolismWork; // -20 <-> 20 // (sans playerBaseMetabolismwork)
    public float playerBaseMaximumMetabolismwork = 20f; //Constante //ce que le metabolisme peut thermoréguler dans les meilleurs conditions
    public float playerMinimumMetabolismWork = -20f;
    public float playerMaximumMetabolismWork = 20f;
    public float playerBaseMetabolismwork = 8f; //Constante //le metabolisme produit par le corp quoi qu'il arrive
    private void Start()
    {
        public int health; // 100 -> 0
        public int hunger; // 100 -> 0
        public int temperature; // [25; 45] < 28 mort, > 42 mort
        public int thirst; // 100 -> 0
        private void Start()
        {

        }
    }
}