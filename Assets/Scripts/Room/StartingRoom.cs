// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace HLO.Room
{
    public class StartingRoom : RoomBase
    {
        protected void Start()
        {
            onEnterRoom?.Invoke();
            SetRoomType(RoomType.Starting);
            ClearRoom();
        }
    }
}