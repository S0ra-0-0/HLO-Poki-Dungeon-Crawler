// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// HLO
using HLO.Room;
using HLO.Door;
using HLO.Layer;

public class TempDungeonSetter : MonoBehaviour
{
    [SerializeField] private Transform dungeon;

    private void Awake()
    {
        RoomBase[] rooms = dungeon.GetComponentsInChildren<RoomBase>();
        foreach (var room in rooms)
        {
            foreach (var door in room.DoorList)
            {
                float angle = 0f;
                switch (door.DoorDirectionType)
                {
                    case DoorDirectionType.Left:
                        angle = 90f;
                        break;
                    case DoorDirectionType.Right:
                        angle = 270f;
                        break;
                    case DoorDirectionType.Top:
                        angle = 0f;
                        break;
                    case DoorDirectionType.Bottom:
                        angle = 180f;
                        break;
                    default:
                        Debug.LogWarning($"{room}'s {door} isn't setting {nameof(DoorDirectionType)}.");
                        break;
                }

                foreach (var col in Physics2D.OverlapBoxAll(door.transform.position, new Vector2(1f, 8f), angle, 1 << LayerDatas.ROOM_LAYER))
                {
                    if (col.transform == door.transform.parent) continue;

                    RoomBase connectedRoom = col.GetComponent<RoomBase>();
                    if (connectedRoom)
                    {
                        door.SetConnectedRoom(connectedRoom);
                    }
                    else
                    {
                        Debug.LogError($"{col.gameObject.name} don't have RoomBase component.");
                    }
                }
            }
        }

        foreach (var room in rooms)
        {
            if (!room.IsDiscovered)
                room.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        GameManager.instance.SetRoomAmount(dungeon.childCount);
    }
}
