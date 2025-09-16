// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class MapUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MapUI otherMap;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (otherMap) otherMap.TurnOnOrOff(true);
        TurnOnOrOff(false);
    }

    public void TurnOnOrOff(bool turning)
    {
        gameObject.SetActive(turning);
    }
}
