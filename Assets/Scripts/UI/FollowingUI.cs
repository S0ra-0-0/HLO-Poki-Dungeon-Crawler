// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

public class FollowingUI : MonoBehaviour
{
    [SerializeField] protected Vector2 posOffset;
    [SerializeField] protected RectTransform rectTransform;

    protected Coroutine currentFollowCoroutine; 

    protected virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void Follow(Transform target, float waitTime)
    {
        if (currentFollowCoroutine != null)
        {
            StopCoroutine(currentFollowCoroutine);
        }

        currentFollowCoroutine = StartCoroutine(FollowCoroutine(target, waitTime));
        
    }

    protected virtual IEnumerator FollowCoroutine(Transform target, float waitTime)
    {
        if (target)
        {
            Vector2 originalPos = rectTransform.anchoredPosition;

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
