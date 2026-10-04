using TM.Player;
using UnityEngine;


public class WorldInfos : MonoBehaviour
{
    public float baseAmbientTemperature;
    [SerializeField] private PlayerHeatCalculation playerHeatCalculation;
    private int count;
    void Update()
    {
        count++;
        if (count > 30)
        {
            count = 0;
            this.gameObject.GetComponent<PlayerInfo>().playerFeelAmbiantTemperature = playerHeatCalculation.CalculateAmbientHeat(this.baseAmbientTemperature);
        }
    }
}

