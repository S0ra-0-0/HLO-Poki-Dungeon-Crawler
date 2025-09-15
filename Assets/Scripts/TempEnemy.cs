// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// HLO
using HLO.Layer;

public class TempEnemy : MonoBehaviour
{
    [SerializeField] private int power;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerDatas.PLAYER_LAYER)
        {
            collision.GetComponent<PlayerHealth>().Hit(power);
        }
    }
}
