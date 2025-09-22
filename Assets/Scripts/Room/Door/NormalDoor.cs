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

        protected override void Awake()
        {
            base.Awake();

            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
        }

        protected override void RegisterRoomAction(RoomBase thisRoom)
        {
            base.RegisterRoomAction(thisRoom);

            thisRoom.RegisterOnEnterRoom(Close);
            thisRoom.RegisterOnClearRoom(() =>
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

            Collider2D other = Physics2D.OverlapBox(transform.position, GetComponent<BoxCollider2D>().size, 0f, 1 << LayerDatas.PLAYER_LAYER);
            if (other)
            {
                connectedRoom.EnterRoom(DoorDirectionType, other.transform);
            }

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