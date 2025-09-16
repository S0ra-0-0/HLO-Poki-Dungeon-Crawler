// System
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;


// Unity
using UnityEngine;

public class MiniMapCameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;

    void Update()
    {
        if (target)
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}
