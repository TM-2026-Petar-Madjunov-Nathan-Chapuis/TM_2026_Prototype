using UnityEngine;

public abstract class HealthManager : MonoBehaviour
{
    public float health; // 0 -> 100

    public abstract void TakeDamage(float ammount, string source);

    public abstract void RestoreHealth(float ammount);

    protected abstract void Die(string source);

    // public void TakeDamage(float ammount)
    //     {
    //         Debug.Log("Player took damamge : " + ammount);
    //         playerHealth-= ammount;
    //         //implement death
    //         if(playerHealth < 0)
    //         {
    //             Debug.Log("PLAYER DEAD");
    //             this.playerHealth = 100;
    //         }
    //     }
    // public void RestoreHealth(float ammount)
    //     {
    //         playerHealth += ammount;
    //         Mathf.Clamp(0, 100, playerHealth);
    //         Debug.Log("Player restored health : " + ammount);
    //     }
}