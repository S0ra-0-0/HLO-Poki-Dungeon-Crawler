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
    public class BossRoomDoor : DoorBase
    {
        [SerializeField] private bool progressComplete;
        [SerializeField] private bool thisRoomCleared;

        protected override void Awake()
        {
            base.Awake();

            Close();
        }

        private void Start()
        {
            Progress.Instance.RegisterOnProgressReward(() =>
            {
                thisRoomCleared = true;
                CheckOpenQualification();
            });
        }

        protected override void Open()
        {
            base.Open();

            GetComponent<SpriteRenderer>().enabled = false;
        }

        protected override void RegisterRoomAction(RoomBase thisRoom)
        {
            base.RegisterRoomAction(thisRoom);

            thisRoom.RegisterOnClearRoom(() =>
            {
                progressComplete = true;
                CheckOpenQualification();
            });
        }

        private void CheckOpenQualification()
        {
            if (progressComplete && thisRoomCleared)
            {
                Open();
            }
        }
    }
}
