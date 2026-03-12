using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public int health; // 100 -> 0
    public int hunger; // 100 -> 0
    public int temperature; // [25; 45] < 28 mort, > 42 mort
    public int thirst; // 100 -> 0
    private void Start()
    {
        
    }
}
