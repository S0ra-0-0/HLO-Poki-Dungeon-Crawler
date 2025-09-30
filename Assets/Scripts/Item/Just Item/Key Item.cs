// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace HLO.Item
{
    public class KeyItem : ItemBase
    {
        public override string Name => "Key";
        public override string Description => "Can open locked things.";
        
        public override void Get(GameObject player)
        {
            player.GetComponent<Inventory>().UpdateKeyAmount(1);
        }
    }
}