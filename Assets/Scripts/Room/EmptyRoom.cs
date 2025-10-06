// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace HLO.Room
{
    public class EmptyRoom : RoomBase
    {
        protected override void Start()
        {
            base.Start();
            
            SetRoomType(RoomType.Empty);
            ClearRoom();
        }
    }
}