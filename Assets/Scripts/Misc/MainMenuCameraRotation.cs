using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainMenuCameraRotation : MonoBehaviour
{
    [SerializeField] private float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(this.transform.up, speed * Time.deltaTime);
    }
}
