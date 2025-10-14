// System
using System;
using System.Collections;
using System.Collections.Generic;
// HLO
using HLO.Door;
using HLO.Layer;
// Unity
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HLO.Room
{
    public enum RoomType
    {
        Normal,
        Starting,
        Tutorial,
        Shop,
        Boss,
        Empty,
    }

    public class RoomBase : MonoBehaviour
    {
        [SerializeField] private RoomType roomType; public RoomType RoomType => roomType;
        [SerializeField] protected List<DoorBase> doorList = new List<DoorBase>(); public List<DoorBase> DoorList => doorList;
        [SerializeField] protected bool isDiscovered; public bool IsDiscovered => isDiscovered;

        protected Action onEnterRoom;
        protected Action onClearRoom;

        protected virtual void Start()
        {
            if(Progress.Instance != null)
                RegisterOnClearRoom(UpdateProgress);
        }

        protected virtual void UpdateProgress() => Progress.Instance.OnProgressUpdated();

        public virtual void SetRoomType(RoomType roomType) => this.roomType = roomType;

        public virtual void RegisterOnEnterRoom(Action action) => onEnterRoom += action;
        public virtual void UnregisterOnEnterRoom(Action action) => onEnterRoom -= action;

        public virtual void RegisterOnClearRoom(Action action) => onClearRoom += action;
        public virtual void UnregisterOnClearRoom(Action action) => onClearRoom -= action;

        public virtual void Discover()
        {
            isDiscovered = true;
            gameObject.SetActive(true);
        }

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
            visitor.gameObject.SetActive(false);
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
            visitor.gameObject.SetActive(false);

            Camera.main.GetComponent<CameraMovement>().Move(transform.position, 0.5f, ()=>
            {
                visitor.gameObject.SetActive(true);
                onEnterRoom?.Invoke();
            });
        }
    }
}