using UnityEngine;
using UnityEngine.Audio;

public class SoundsManager : MonoBehaviour
{
    [Header("Fox Sounds")]
    public AudioClip foxSnowStep;
    public AudioClip foxRockStep;
    public AudioClip foxHitSound;
    public AudioClip foxRunSqueak;
    [Header("Environement Sounds")]
    public AudioClip treeFall;
    public AudioClip fireBurning;
    public AudioClip fireWoosh;
    [Header("Player Sounds")]
    public AudioClip playerPlaceLog;
    public AudioClip playerSnowStep;
    public AudioClip playerRockStep;
    public AudioClip playerAxeSwing;
    public AudioClip playerCanDrink;
    public AudioClip playerCanThrow;
    public AudioClip playerSteakEat;
    public AudioClip playerGrab;
    public AudioClip playerDrop;
    public AudioClip playerButtonSelect;
    public AudioClip playerInventoryItemGrab;
    public AudioClip playerInventoryItemDrop;
    [Header("Miscallenous")]
    public static SoundsManager Instance;
    
    void Awake() // attempt at a "singleton" but MUST BE SET IN THE SCRIPT ORDERING AT FIRST IN UNITY SETTINGS BECAUSE THIS IS BEING CALLED IN SOME ONENABLE() SCRIPTS
    {
        if (Instance != null && Instance != this) //avoid multiple instances
        {
            Destroy(gameObject);
            return;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
 