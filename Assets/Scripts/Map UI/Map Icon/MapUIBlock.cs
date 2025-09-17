// System
using HLO.Room;




// Unity
using UnityEngine;

public class MapUIBlock : MonoBehaviour
{
    private RoomBase room;

    private void Awake()
    {
        room = GetComponentInParent<RoomBase>();
        if (room)
        {
            room.RegisterOnEnterRoom(Erase);
        }
    }

    private void Erase()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        room.UnregisterOnEnterRoom(Erase);
    }
}
