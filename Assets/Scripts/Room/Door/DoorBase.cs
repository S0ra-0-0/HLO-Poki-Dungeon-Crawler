// Unity
using UnityEngine;

// HLO
using HLO.Room;
using HLO.Layer;

namespace HLO.Door
{
    public enum DoorDirectionType
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right= 8
    }

    public class DoorBase : MonoBehaviour
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

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            if (IsOpen && other.gameObject.layer == LayerDatas.PLAYER_LAYER)
            {
                connectedRoom.EnterRoom(DoorDirectionType, other.transform);
            }
        }

        public virtual void SetConnectedRoom(RoomBase room) => connectedRoom = room;
        protected virtual void Open() => isOpen = true;
        protected virtual void Close() => isOpen = false;

        protected virtual void DiscoverConnectedRoom()
        {
            if(!connectedRoom.IsDiscovered) connectedRoom.Discover();
        }
    }
}
