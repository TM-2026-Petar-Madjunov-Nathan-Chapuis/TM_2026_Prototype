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
        [SerializeField] public GameObject particlesSystem;
        [SerializeField] int leavesParticlesCount;
        [SerializeField] float leavesTimerSeconds;
        [SerializeField] float treeFallDirectionFactor;
        [SerializeField, Range(1, 10)]
        private int UpperLootBound;
        [SerializeField, Range(1, 10)]
        private int LowerLootBound;
        public void Fall(InventoryManager inventoryManager, Vector3 playerPosition)
        {
            Vector3 direction = Vector3.Normalize(this.transform.position - playerPosition);
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction).normalized; //gets the axis around wich the logs should rotate to face away from the player and the tree to fall corectly
            
            float objectHeight = this.droppedWorldItemPrefabs.GetComponentInChildren<MeshRenderer>().bounds.size.y;

            int count = UnityEngine.Random.Range(LowerLootBound, UpperLootBound+1);
            for(int i = 0; i < count; i++)
            {
                GameObject log = GameObject.Instantiate(droppedWorldItemPrefabs, this.transform.parent);

                float logHeight = this.transform.position.y + (objectHeight * i) + 0.5f * i + 1.5f;
                float posOffsetMultiplier = treeFallDirectionFactor * (Mathf.Sqrt(i)*i + 1);
                Vector2 logPos = new Vector2(this.transform.position.x + direction.x  * posOffsetMultiplier, this.transform.position.z + direction.z * posOffsetMultiplier);

                log.transform.position = new Vector3(logPos.x, logHeight, logPos.y);

                float angle = Mathf.Sqrt(i) * 15f; //degrees away from the player at each log
                log.transform.rotation = Quaternion.AngleAxis(angle, rotationAxis);

                log.GetComponent<Rigidbody>().AddForce(new Vector3(direction.x, 0, direction.z).normalized * 2f, ForceMode.VelocityChange);//addforce for nice effect
            }

            GameObject particles = GameObject.Instantiate(particlesSystem, this.transform.parent);
            particles.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 10, this.transform.position.z);
            particles.GetComponent<TreeDroppedLeaves>().Emit(leavesParticlesCount, leavesTimerSeconds);
            GameObject.Destroy(this.gameObject);
        }

    }
}
