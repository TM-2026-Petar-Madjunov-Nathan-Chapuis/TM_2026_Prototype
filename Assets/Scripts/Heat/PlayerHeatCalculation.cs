using System;
using System.Collections.Generic;
using TM.Player;
using UnityEngine;

public class PlayerHeatCalculation : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private IHeatObject[] heatGeneratingObjects;
    [SerializeField] private float maxHeat;

    void Start()
    {
        List<IHeatObject> list = new List<IHeatObject>();
        MonoBehaviour[] monoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (MonoBehaviour monobehavior in monoBehaviours)
        {
            if (monobehavior is IHeatObject iheat)
            {
                list.Add(iheat);
            }
        }
        heatGeneratingObjects = list.ToArray();
    }

    public float CalculateAmbientHeat(float baseAmbientTemperature)
    {
        Vector3 playerPos = this.player.transform.position;

        float heat = 0f;
        foreach (IHeatObject heatObject in heatGeneratingObjects)
        {
            Vector3 offset = heatObject.gameObject.transform.position - playerPos;

            heat += heatObject.temp / (1f + offset.sqrMagnitude); //basically distance squared disatnce*distance
        }
        heat = Math.Clamp(heat, 0, maxHeat);
        return baseAmbientTemperature + heat;
    }
}
