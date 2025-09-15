// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// HLO
using HLO.Door;

namespace HLO.Room
{
    public enum RoomType
    {
        Normal,
        Shop,
        Boss
    }

    public class RoomBase : MonoBehaviour
    {
        [SerializeField] private RoomType roomType; public RoomType RoomType => roomType;
        [SerializeField] protected List<DoorBase> doorList = new List<DoorBase>();

        protected Action onEnterRoom;
        protected Action onClearRoom;

        public virtual void SetRoomType(RoomType roomType) => this.roomType = roomType;

        public virtual void RegisterOnEnterRoom(Action action) => onEnterRoom += action;
        public virtual void UnregisterOnEnterRoom(Action action) => onEnterRoom -= action;

        public virtual void RegisterOnClearRoom(Action action) => onClearRoom += action;
        public virtual void UnregisterOnClearRoom(Action action) => onClearRoom -= action;

        public virtual void EnterRoom()
        {
            onEnterRoom?.Invoke();
        }

        public virtual void ClearRoom()
        {
            onClearRoom?.Invoke();
        }
    }
}