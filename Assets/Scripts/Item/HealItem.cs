// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace HLO.Item
{
    public class HealItem : DisplayedItem
    {
        [SerializeField] private int healAmount = 1;

        public override void Use(GameObject player)
        {
            player.GetComponent<PlayerHealth>().HealHP(healAmount);
        }
    }
}