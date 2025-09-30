// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace HLO.Item
{
    public class CoinItem : ItemBase
    {
        public override string Name => "Coin";

        public override string Description
        {
            get
            {
                if (coinAmount == 1)
                {
                    return "Get a coin.";
                }
                else
                {
                    return $"Get {coinAmount} coins.";
                }
            }
        }

        [SerializeField] private int coinAmount = 1;

        public override void Get(GameObject player)
        {
            player.GetComponent<Inventory>().UpdateCoins(coinAmount);
        }
    }
}