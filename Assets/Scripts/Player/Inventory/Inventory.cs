// System
using System;
using System.Collections.Generic;

// Unity
using UnityEngine;

// TMP
using TMPro;
using HLO.Item;

public class Inventory : MonoBehaviour
{
    private const int MAX_AMOUNT = 99;
    private const int MIN_AMOUNT = 0;

    [Header("Follow Items")]
    [SerializeField] private List<FollowingItem> followItemList = new List<FollowingItem>();

    [Header("Coins")]
    [SerializeField] private TMP_Text textCoins;
    [SerializeField] private int coins = 0; public int Coins => coins;

    #region Coins
    public bool UseCoins(int necessaryCoins)
    {
        if (Coins >= necessaryCoins)
        {
            UpdateCoins(-necessaryCoins);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateCoins(int getCoins)
    {
        if (!textCoins) return;
        
        coins = Mathf.Clamp(coins + getCoins, MIN_AMOUNT, MAX_AMOUNT);
        textCoins.text = coins.ToString("D2");
    }

#if UNITY_EDITOR
    [ContextMenu("Give Coins")]
    public void GiveCoins() => UpdateCoins(10);
#endif
    #endregion

    #region Follow Items
    public void AddFollowItem(FollowingItem followItem)
    {
        followItem.SetTarget(GetTarget());
        followItemList.Add(followItem);
    }

    private Transform GetTarget()
    {
        if (followItemList.Count == 0)
        {
            return transform;
        }
        else
        {
            return followItemList[followItemList.Count - 1].transform;
        }
    }

    public bool UseItem(Type itemType)
    {
        for (int i = 0; i < followItemList.Count; i++)
        {
            ItemBase item = followItemList[i].Item;

            if (item.GetType() == itemType)
            {
                if (i + 1 < followItemList.Count)
                {
                    followItemList[i + 1].SetTarget(followItemList[i].Target);
                }

                followItemList.RemoveAt(i);

                item.Use(gameObject);
                return true;
            }
        }

        return false;
    }
    #endregion
}
