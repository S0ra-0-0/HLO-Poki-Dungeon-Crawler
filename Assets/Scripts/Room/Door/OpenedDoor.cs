// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// HLO
using HLO.Layer;

namespace HLO.Door
{
    public class OpenedDoor : DoorBase
    {
        protected override void Awake()
        {
            base.Awake();

            Open();
        }

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == LayerDatas.PLAYER_LAYER)
            {
                connectedRoom.EnterRoom(DoorDirectionType, other.transform);
            }
        }

        protected override void Open()
        {
            base.Open();

            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}