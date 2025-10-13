// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;
using UnityEngine.UI;

public class FollowingLayoutUI : FollowingUI
{
    [SerializeField] private LayoutGroup layoutGroup;

    [SerializeField] private TextAnchor originalAlignment;
    [SerializeField] private TextAnchor followingAlignment;

    protected override void Start()
    {
        base.Start();

        layoutGroup = GetComponent<LayoutGroup>();
        originalAlignment = layoutGroup.childAlignment;
    }

    protected override IEnumerator FollowCoroutine(Transform target, float waitTime)
    {
        if (target)
        {
            layoutGroup.childAlignment = followingAlignment;

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
            layoutGroup.childAlignment = originalAlignment;
        }
        
        currentFollowCoroutine = null;
    }
}
