// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace HLO.Item
{
    public class MaxHPUpItem : DisplayedItem
    {
        [SerializeField] private GameObject heart;

        public override void Get(GameObject player)
        {
            player.GetComponent<PlayerHealth>().AddHeart(heart);
        }
    }
}