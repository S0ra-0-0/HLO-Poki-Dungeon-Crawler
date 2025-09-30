// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace HLO.Item
{
    public class MaxHPUpItem : ItemBase
    {
        public override string Name => "Max HP Up";
        public override string Description => "Increases the heart by one space.";

        [SerializeField] private GameObject heart;
        public override void Get(GameObject player)
        {
            player.GetComponent<PlayerHealth>().AddHeart(heart);
        }
    }
}