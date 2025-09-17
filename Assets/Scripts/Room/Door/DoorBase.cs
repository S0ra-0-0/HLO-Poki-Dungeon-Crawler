// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// HLO
using HLO.Room;

namespace HLO.Door
{
    public enum DoorDirectionType
    {
        None,
        Top,
        Bottom,
        Left,
        Right
    }

    public abstract class DoorBase : MonoBehaviour
    {
        [SerializeField] protected RoomBase connectedRoom; public RoomBase ConnectedRoom => connectedRoom;
        [SerializeField] protected DoorDirectionType doorDirectionType; public DoorDirectionType DoorDirectionType => doorDirectionType;
        [SerializeField] protected bool isOpen; public bool IsOpen => isOpen;

        protected virtual void Awake()
        {
            RegisterRoomAction(transform.parent.GetComponent<RoomBase>());
        }

        protected virtual void RegisterRoomAction(RoomBase thisRoom)
        {
            thisRoom.RegisterOnEnterRoom(DiscoverConnectedRoom);
            thisRoom.RegisterOnClearRoom(() =>
            {
                thisRoom.UnregisterOnEnterRoom(DiscoverConnectedRoom);
            });
        }

        protected abstract void OnCollisionEnter2D(Collision2D other);

        public virtual void SetConnectedRoom(RoomBase room) => connectedRoom = room;
        protected virtual void Open() => isOpen = true;
        protected virtual void Close() => isOpen = false;

        protected virtual void DiscoverConnectedRoom()
        {
            if(!connectedRoom.IsDiscovered) connectedRoom.Discover();
        }
    }
}
