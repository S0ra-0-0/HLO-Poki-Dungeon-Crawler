// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

public class FollowingUI : MonoBehaviour
{
    [SerializeField] private Vector2 posOffset;
    [SerializeField] private Vector2 originalPos;
    [SerializeField] private RectTransform rectTransform;

    private Coroutine currentFollowCoroutine; 

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;
    }

    public void Follow(Transform target, float waitTime)
    {
        if (currentFollowCoroutine != null)
        {
            StopCoroutine(currentFollowCoroutine);
        }

        currentFollowCoroutine = StartCoroutine(FollowCoroutine(target, waitTime));
        
    }

    private IEnumerator FollowCoroutine(Transform target, float waitTime)
    {
        if (target)
        {
            float timer = 0f;
            while (enabled && timer < waitTime)
            {
                rectTransform.position = (Vector2)Camera.main.WorldToScreenPoint(target.position) + posOffset;
                timer += Time.deltaTime;

                yield return null;
            }

            rectTransform.anchoredPosition = originalPos;
        }
        
        currentFollowCoroutine = null;
    }
}
