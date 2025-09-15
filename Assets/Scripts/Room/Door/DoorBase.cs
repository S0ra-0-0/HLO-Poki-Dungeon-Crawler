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
        Top,
        Bottom,
        Left,
        Right
    }

    public abstract class DoorBase : MonoBehaviour
    {
        [SerializeField] protected RoomBase ConnectedRoom;
        [SerializeField] protected DoorDirectionType doorDirectionType; public DoorDirectionType DoorDirectionType => doorDirectionType;
        [SerializeField] protected bool isOpen; public bool IsOpen => isOpen;

        protected abstract void OnCollisionEnter2D(Collision2D other);
    }
}
