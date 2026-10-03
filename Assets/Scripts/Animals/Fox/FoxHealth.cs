using UnityEngine;

public class FoxHealth : HealthManager
{
    public GameObject foxDollPrefab;
    public override void TakeDamage(float ammount, string source)
    {
        Debug.Log("Fox took damage : " + ammount + " by " + source + " they had " + this.health);
        this.health -= ammount;
        if (this.health < 0)
        {
            this.Die(source);
        }
    }
    void Start()
    {
        this.health = 100; 
    }
    public override void RestoreHealth(float ammount) //will never be used
    {
        Debug.Log("Fox restored health : " + ammount);
        this.health += ammount;
        Mathf.Clamp(0, 100, health);
    }

    protected override void Die(string source)
    {
        Debug.Log("Fox died by " + source);
        GameObject doll = GameObject.Instantiate(foxDollPrefab, this.transform.parent);
        doll.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
        doll.GetComponent<Rigidbody>().AddForceAtPosition(doll.transform.position + new Vector3(0, 0.1f, 0), this.transform.up * -1000);
        Destroy(this.gameObject);
    }
}