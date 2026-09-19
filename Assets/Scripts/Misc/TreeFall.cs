using System;
using TM.Inventory;
using TM.Items;
using Unity.AppUI.UI;
using UnityEngine;

namespace TM.Misc
{
    public class TreeFall : MonoBehaviour
    {
        [SerializeField] private GameObject droppedWorldItemPrefabs;
        [SerializeField] private GameObject particlesSystem;
        [SerializeField] int leavesParticlesCount;
        [SerializeField] float leavesTimerSeconds;
        [SerializeField] float treeFallDirectionFactor;
        [SerializeField, Range(1, 5)]
        private int UpperLootBound;
        [SerializeField, Range(1, 5)]
        private int LowerLootBound;
        public void Fall(InventoryManager inventoryManager, Vector3 playerPosition)
        {
            Vector3 direction = Vector3.Normalize(this.transform.position - playerPosition);
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction).normalized; //gets the axis around wich the logs should rotate to face away from the player and the tree to fall corectly
            float height = this.GetComponent<MeshRenderer>().bounds.size.y;
            float objectHeight = this.droppedWorldItemPrefabs.GetComponentInChildren<MeshRenderer>().bounds.size.y;
            int count = UnityEngine.Random.Range(LowerLootBound, UpperLootBound+1);
            float step = height / objectHeight;
            for(int i = 0; i < count; i++)
            {
                GameObject log = GameObject.Instantiate(droppedWorldItemPrefabs);
                log.GetComponent<WorldItem>().inventoryManager = inventoryManager;
                log.transform.position = new Vector3(this.transform.position.x + direction.x * treeFallDirectionFactor * (Mathf.Sqrt(i)*i + 1), this.transform.position.y + (objectHeight * i) + 0.5f * i + 1.5f, this.transform.position.z + direction.z * treeFallDirectionFactor * (Mathf.Sqrt(i)*i + 1));
                float angle = Mathf.Sqrt(i) * 15f; //degrees away from the player at each log
                log.transform.rotation = Quaternion.AngleAxis(angle, rotationAxis);
                log.GetComponent<Rigidbody>().AddForce(new Vector3(direction.x, 0, direction.z).normalized * 2f, ForceMode.VelocityChange);
                }
            GameObject particles = GameObject.Instantiate(particlesSystem);
            particles.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 10, this.transform.position.z);
            particles.GetComponent<TreeDroppedLeaves>().Emit(leavesParticlesCount, leavesTimerSeconds);
            GameObject.Destroy(this.gameObject);
        }
    }
}
