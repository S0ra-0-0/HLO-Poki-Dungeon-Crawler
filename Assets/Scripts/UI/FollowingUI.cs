// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

public class FollowingUI : MonoBehaviour
{
    [SerializeField] protected Vector2 posOffset = Vector2.down;
    [SerializeField] protected Vector2 originalPos;
    [SerializeField] protected RectTransform rectTransform;

    protected Coroutine currentFollowCoroutine; 

    protected virtual void Start()
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

    protected virtual IEnumerator FollowCoroutine(Transform target, float waitTime)
    {
        if (target)
        {
            float timer = 0f;
            while (enabled && timer < waitTime)
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, (Vector2)target.position + posOffset);
                // RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, null, out Vector2 point);
                rectTransform.position = screenPoint;

                timer += Time.deltaTime;

                yield return null;
            }

            rectTransform.anchoredPosition = originalPos;
        }
        
        currentFollowCoroutine = null;
    }
}
