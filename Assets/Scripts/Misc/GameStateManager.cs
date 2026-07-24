
using TM.Player;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerCamera playerCamera;


    public static GameStateManager Instance { get; private set; }
    void Awake() // attempt at a "singleton" but MUST BE SET IN THE SCRIPT ORDERING AT FIRST IN UNITY SETTINGS BECAUSE THIS IS BEING CALLED IN SOME ONENABLE() SCRIPTS
    {
        if (Instance != null && Instance != this) //avoid multiple instances
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void FreezeTime()
    {
        Time.timeScale = 0f;
        playerCamera.lookingAllowed = false;
    }
    public void UnFreezeTime()
    {
        Time.timeScale = 1f;
        playerCamera.lookingAllowed = true;
    }
    public void SlowTime()
    {
        Time.timeScale = 0.3f;
    }
    public void FreezeLooking()
    {
        playerCamera.lookingAllowed = false;
    }
    public void UnFreezeLooking()
    {
        playerCamera.lookingAllowed = true; 
    }
}