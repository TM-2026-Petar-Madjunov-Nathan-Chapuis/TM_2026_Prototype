using UnityEngine;

public class SoundsManager : MonoBehaviour
{
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
