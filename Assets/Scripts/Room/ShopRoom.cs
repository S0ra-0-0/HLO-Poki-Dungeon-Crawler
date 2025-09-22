// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace HLO.Room
{
    public class ShopRoom : RoomBase
    {
        protected void Start()
        {
            SetRoomType(RoomType.Shop);
            RegisterOnEnterRoom(ClearRoom);
            RegisterOnClearRoom(() => { UnregisterOnEnterRoom(ClearRoom); });
        } 
    }
}