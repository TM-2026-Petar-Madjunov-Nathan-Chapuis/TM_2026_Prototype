using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public float playerHealth; // 100 -> 0
    public float playerHunger; // 100 -> 0
    public float playerThirst; // 100 -> 0
    public float playerCorporalTemperature; // [25; 45] < 28 mort, > 42 mort
    public float playerNormalCorporalTemperature = 36.5f; //Constante
    public float playerThermalBlilan;
    public float playerClothesResistance = 0f; // 0 -> 1
    public float playerMetabolismWork; // -20 <-> 20
    public float playerBaseMaximumMetabolismwork = 20f; //Constante //ce que le metabolisme peut thermoréguler dans les meilleurs conditions
    public float playerMinimumMetabolismWork = -20f;
    public float playerMaximumMetabolismWork = 20f;
    public float playerBaseMetabolismwork = 8f; //Constante //le metabolisme produit par le corp quoi qu'il arrive
    private void Start()
    {
        
    }
}
