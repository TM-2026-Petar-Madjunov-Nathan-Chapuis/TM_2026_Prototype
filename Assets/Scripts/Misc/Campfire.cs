using TM.Inventory;
using TM.Items;
using TM.Player;
using UnityEngine;

public class Campfire : MonoBehaviour, IInteractable
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Mesh onelog;
    [SerializeField] private Mesh twologs;
    [SerializeField] private Mesh threelogs;
    [SerializeField] private Mesh fourlogs;
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private GameObject meatPrefab;
    [SerializeField] private Transform transformParent;
    public InteractionType GetInteractionType() => InteractionType.Fuel;
    private CampfireState campfireState = CampfireState.Empty;
    public void Interact(GameObject player)
    {
        playerItemController playerItemController = player.GetComponent<playerItemController>();
        if (playerItemController.heldItemGameobject?.GetComponent<HeldItem>().itemData.GetItemType() == ItemType.Combustible)//clearly UGLY
        {
            player.GetComponent<InventoryManager>().DestroyItem(playerItemController.heldItem);
            GameObject.Destroy(playerItemController.heldItemGameobject);
            this.IncrementFire();
        }
        if (playerItemController.heldItemGameobject?.GetComponent<HeldItem>().itemData.GetItemType() == ItemType.RawMeat)
        {
            if (this.campfireState == CampfireState.Lit)
            {
                GameObject meat = GameObject.Instantiate(meatPrefab, transformParent);
                meat.transform.position = this.transform.position + this.transform.up * 1.5f;
                meat.GetComponent<Rigidbody>().AddForceAtPosition(meat.transform.position + new Vector3(0, 0.1f, 0), this.transform.up * -1000);

                player.GetComponent<InventoryManager>().DestroyItem(playerItemController.heldItem);
                GameObject.Destroy(playerItemController.heldItemGameobject);
            }
        }
    }
    private void IncrementFire()
    {
        if (this.campfireState == CampfireState.Empty)
        {
            this.campfireState = CampfireState.OneLog;
            this.meshFilter.mesh = onelog;
        }
        else if (this.campfireState == CampfireState.OneLog)
        {
            this.campfireState = CampfireState.TwoLogs;
            this.meshFilter.mesh = twologs;
        }
        else if (this.campfireState == CampfireState.TwoLogs)
        {
            this.campfireState = CampfireState.ThreeLogs;
            this.meshFilter.mesh = threelogs;
        }
        else if (this.campfireState == CampfireState.ThreeLogs)
        {
            this.campfireState = CampfireState.Lit;
            var emmision = this.fireParticles.emission;
            emmision.enabled = true;
            this.meshFilter.mesh = fourlogs;
        }
        else if (this.campfireState == CampfireState.Lit)
        {
            this.EmitBurst();
        }
    }

    private void EmitBurst()
    {
        for (int particleIndex = 0; particleIndex < 36; particleIndex++)
        {
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
            {
                position = new Vector3(
                    Random.Range(-0.6f, 0.6f),
                    Random.Range(0f, 0.25f),
                    Random.Range(-0.6f, 0.6f)),
                velocity = (Random.insideUnitSphere + Vector3.up * 1.5f).normalized * Random.Range(8f, 10f), //random upwards direction and velocity
                startLifetime = Random.Range(0.4f, 0.9f),
                startSize = Random.Range(1f, 1.5f)
            };

            this.fireParticles.Emit(emitParams, 1);
        }
    }
    void Start()
    {
        this.meshFilter.mesh = null;
        var emission = this.fireParticles.emission;
        emission.enabled = false;
    }
}
public enum CampfireState
{
    Empty,
    OneLog,
    TwoLogs,
    ThreeLogs,
    FourLogs,
    Lit,
}