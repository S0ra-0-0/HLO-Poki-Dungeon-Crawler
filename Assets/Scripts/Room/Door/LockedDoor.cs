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
    public class LockedDoor : DoorBase
    {
        [SerializeField] private int necessaryKeyAmount = 1;

        protected override void Awake()
        {
            Close();
        }

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == LayerDatas.PLAYER_LAYER)
            {
                if (IsOpen)
                {
                    connectedRoom.EnterRoom(DoorDirectionType, other.transform);
                }
                else if (other.gameObject.GetComponent<Inventory>().UseKeys(necessaryKeyAmount))
                {
                    Open();
                }
            }
        }

        protected override void Open()
        {
            base.Open();

            Collider2D other = Physics2D.OverlapBox(transform.position, GetComponent<BoxCollider2D>().size, 0f, 1 << LayerDatas.PLAYER_LAYER);
            if (other)
            {
                connectedRoom.EnterRoom(DoorDirectionType, other.transform);
            }

            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}