// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// TMP
using TMPro;

public class Inventory : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text textKeyAmount;

    [Header("Keys")]
    [SerializeField] private int keyAmount = 0;

    public bool UseKeys(int necessaryKeyAmount)
    {
        if (keyAmount >= necessaryKeyAmount)
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
        keyAmount += getKeyAmount;
        textKeyAmount.text = keyAmount.ToString("D2");
    }

    [ContextMenu("Give A Key")]
    private void GiveAKey() => UpdateKeyAmount(1);
}
