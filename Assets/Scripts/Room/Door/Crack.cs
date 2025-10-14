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
    public class Crack : DoorBase
    {
        [SerializeField] private Sprite wallOpenSprite;

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

        protected override void Open()
        {
            base.Open();

            GetComponent<SpriteRenderer>().sprite = wallOpenSprite;

            Collider2D other = Physics2D.OverlapBox(transform.position, GetComponent<BoxCollider2D>().size, 0f, 1 << LayerDatas.PLAYER_LAYER);
            if (other)
            {
                connectedRoom.EnterRoom(DoorDirectionType, other.transform);
            }
        }
    }
}