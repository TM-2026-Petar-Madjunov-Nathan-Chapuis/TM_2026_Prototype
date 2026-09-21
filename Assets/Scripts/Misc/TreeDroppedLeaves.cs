using System;
using UnityEngine;

public class TreeDroppedLeaves : MonoBehaviour
{
    private float? timerSeconds;
    [SerializeField] private ParticleSystem leavesParticlesSystem;
    void Update()
    {
        if (timerSeconds != null)
        {
            timerSeconds -= Time.deltaTime;
        }
        if (timerSeconds != null && timerSeconds <= 0)
        {
            leavesParticlesSystem.Clear();
            GameObject.Destroy(this.gameObject);
        }
    }
    public void Emit(int count, float timerSeconds)
    {
        this.timerSeconds = timerSeconds;
        leavesParticlesSystem.Emit(count);
    }
}