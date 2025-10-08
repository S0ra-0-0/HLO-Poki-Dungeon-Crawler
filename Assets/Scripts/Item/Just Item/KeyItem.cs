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
            FollowItem followItem = gameObject.AddComponent<FollowItem>();
            player.GetComponent<Inventory>().AddFollowItem(followItem);

            GetComponent<DroppedItem>().KillMe();
        }
        
        public override void Use(GameObject player)
        {
            Destroy(gameObject);
        }
    }
}