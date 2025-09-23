// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// TMP
using TMPro;
using NUnit.Framework.Constraints;

public class Inventory : MonoBehaviour
{
    private const int MAX_AMOUNT = 99;
    private const int MIN_AMOUNT = 0;

    [Header("Keys")]
    [SerializeField] private TMP_Text textKeyAmount;
    [SerializeField] private int keyAmount = 0; public int KeyAmount => keyAmount;

    [Header("Coins")]
    [SerializeField] private TMP_Text textCoins;
    [SerializeField] private int coins = 0; public int Coins => coins;

    #region Keys
    public bool UseKeys(int necessaryKeyAmount)
    {
        if (KeyAmount >= necessaryKeyAmount)
        {
            UpdateKeyAmount(-necessaryKeyAmount);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateKeyAmount(int getKeyAmount)
    {
        keyAmount = Mathf.Clamp(KeyAmount + getKeyAmount, MIN_AMOUNT, MAX_AMOUNT);
        textKeyAmount.text = keyAmount.ToString("D2");
    }

    [ContextMenu("Give A Key")]
    private void GiveAKey() => UpdateKeyAmount(1);
    #endregion

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

    private void UpdateCoins(int getCoins)
    {
        coins = Mathf.Clamp(coins + getCoins, MIN_AMOUNT, MAX_AMOUNT);
        textCoins.text = coins.ToString("D2");
    }

    [ContextMenu("Give Coins")]
    private void GiveCoins() => UpdateCoins(10);
    #endregion
}
