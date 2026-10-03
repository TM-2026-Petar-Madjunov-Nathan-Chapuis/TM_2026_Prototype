using UnityEngine;

public class PlayerSnowParticles : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    private ParticleSystem snowParticleSystem;
    public int particleEmitedCount = 5;

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
        if (characterController.isGrounded && characterController.velocity.magnitude > 0)   snowParticleSystem.Emit(particleEmitedCount);   
    }

}
