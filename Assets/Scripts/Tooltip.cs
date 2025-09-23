// System
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;


// Unity
using UnityEngine;

// TMP
using TMPro;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance { get; private set; }

    [SerializeField] private bool isUsed;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameObject.SetActive(false);
    }

    public void Rent(Vector2 worldPosition, string name, string description)
    {
        if (isUsed) return;

        nameText.text = name;
        descriptionText.text = description;

        transform.position = Camera.main.WorldToScreenPoint(worldPosition);

        isUsed = true;
        gameObject.SetActive(true);
    }

    public void Return()
    {
        isUsed = false;
        gameObject.SetActive(false);
    }
}
