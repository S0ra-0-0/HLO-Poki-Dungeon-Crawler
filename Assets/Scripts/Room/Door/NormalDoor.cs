// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// HLO
using HLO.Layer;
using HLO.Room;

namespace HLO.Door
{
    public class NormalDoor : DoorBase
    {
        SpriteRenderer spriteRenderer;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            RoomBase thisRoom = transform.parent.GetComponent<RoomBase>();
            thisRoom.RegisterOnEnterRoom(Close);
            thisRoom.RegisterOnClearRoom(()=>
            {
                Open();
                thisRoom.UnregisterOnEnterRoom(Close);
            });
        }

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (IsOpen && other.gameObject.layer == LayerDatas.PLAYER_LAYER)
            {
                connectedRoom.EnterRoom(DoorDirectionType, other.transform);
            }
        }

        protected override void Open()
        {
            base.Open();

            // TODO: Animation will be added later
            spriteRenderer.enabled = false;
        }

        protected override void Close()
        {
            base.Close();

            // TODO: Animation will be added later
            spriteRenderer.enabled = true;
        }
    }
}