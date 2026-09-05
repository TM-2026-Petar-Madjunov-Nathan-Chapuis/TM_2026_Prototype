using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class AnimalSnowParticles : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    public int particleEmitedCount = 5;
    private ParticleSystem snowParticleSystem;
    void Awake()
    {
        snowParticleSystem = this.GetComponent<ParticleSystem>();
        if (snowParticleSystem == null) throw new System.Exception("No ParticleSystem Found");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (navMeshAgent.isOnNavMesh && navMeshAgent.velocity.magnitude > 0)   snowParticleSystem.Emit(particleEmitedCount);   
    }
}
