// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// HLO
using HLO.Door;
using TreeEditor;

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
        [SerializeField] protected List<DoorBase> doorList = new List<DoorBase>(); public List<DoorBase> DoorList => doorList;

        protected Action onEnterRoom;
        protected Action onClearRoom;

        public virtual void SetRoomType(RoomType roomType) => this.roomType = roomType;

        public virtual void RegisterOnEnterRoom(Action action) => onEnterRoom += action;
        public virtual void UnregisterOnEnterRoom(Action action) => onEnterRoom -= action;

        public virtual void RegisterOnClearRoom(Action action) => onClearRoom += action;
        public virtual void UnregisterOnClearRoom(Action action) => onClearRoom -= action;

        public virtual void EnterRoom(DoorDirectionType prevDoorDirection, Transform visitor)
        {
            SetVisitorPosition(prevDoorDirection, visitor);
        }

        [ContextMenu("ClearRoom")]
        public virtual void ClearRoom()
        {
            onClearRoom?.Invoke();
            onClearRoom = null;
        }

        protected virtual void SetVisitorPosition(DoorDirectionType prevDoorDirection, Transform visitor)
        {
            Vector3 newVisitorPos = visitor.position;

            switch (prevDoorDirection)
            {
                case DoorDirectionType.Top:
                    newVisitorPos.y += 6.5f;
                    break;
                case DoorDirectionType.Bottom:
                    newVisitorPos.y -= 6.5f;
                    break;
                case DoorDirectionType.Left:
                    newVisitorPos.x -= 5f;
                    break;
                case DoorDirectionType.Right:
                    newVisitorPos.x += 5f;
                    break;
                default:
                    newVisitorPos = transform.position;
                    break;
            }

            visitor.position = newVisitorPos;
            Camera.main.GetComponent<CameraMovement>().Move(transform.position, 0.5f, onEnterRoom);
        }
    }
}