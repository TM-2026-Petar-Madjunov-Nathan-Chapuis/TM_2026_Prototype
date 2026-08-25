using UnityEngine;

public class FollowParentPositionOnly : MonoBehaviour
{
    Vector3 initoffset;
    [SerializeField] GameObject parent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.initoffset = this.transform.position - parent.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = parent.transform.position + this.initoffset;    
    }
}
