// System
using System;
using System.Collections;
using System.Collections.Generic;
using HLO.Item;


// Unity
using UnityEngine;

namespace HLO.Item
{
    public class FollowItem : MonoBehaviour
    {
        [Header("Item")]
        [SerializeField] private ItemBase item;
        public ItemBase Item => item;

        [Header("Follow")]
        [SerializeField] private int followDelayFrame = 6;
        [SerializeField] private Transform target; public Transform Target => target;
        [SerializeField] private Queue<Vector2> targetPosQueue = new Queue<Vector2>();

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void Start()
        {
            item = GetComponent<ItemBase>();
        }

        private void Update()
        {
            Follow();
        }

        private void Follow()
        {
            if (!target) { return; }

            if (!targetPosQueue.Contains(target.position))
            {
                targetPosQueue.Enqueue(target.position);
            }

            if (targetPosQueue.Count > followDelayFrame)
            {
                transform.position = targetPosQueue.Dequeue();
            }
            else if (targetPosQueue.Count < followDelayFrame)
            {
                transform.position = Vector2.Lerp(transform.position, target.position, (float)targetPosQueue.Count / followDelayFrame);
            }
        }
    }
}