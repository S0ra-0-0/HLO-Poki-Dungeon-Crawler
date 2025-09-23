// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// HLO
using HLO.Layer;

namespace HLO.Door
{
    public class LockedDoor : DoorBase
    {
        [SerializeField] private int necessaryKeyAmount = 1;
        [SerializeField] private Lock _lock;

        protected override void Awake()
        {
            base.Awake();

            Close();
        }

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == LayerDatas.PLAYER_LAYER)
            {
                if (IsOpen)
                {
                    connectedRoom.EnterRoom(DoorDirectionType, other.transform);
                }
                else if (other.gameObject.GetComponent<Inventory>().UseKeys(necessaryKeyAmount))
                {
                    _lock.Unlock();
                    StartCoroutine(EnterWaitCoroutine(other.transform));
                }
            }
        }

        private IEnumerator EnterWaitCoroutine(Transform playerTransform)
        {
            playerTransform.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            _lock.Hide();
            GetComponent<SpriteRenderer>().enabled = false;

            Open();

            connectedRoom.EnterRoom(DoorDirectionType, playerTransform);
        }
    }
}