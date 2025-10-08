// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// HLO
using HLO.Layer;

namespace HLO.Door
{
    public class OpenedDoor : DoorBase
    {
        protected override void Awake()
        {
            base.Awake();

            Open();
        }

        protected override void Open()
        {
            base.Open();

            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}