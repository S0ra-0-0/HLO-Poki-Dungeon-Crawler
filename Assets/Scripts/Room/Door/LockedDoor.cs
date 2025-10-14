// System
using System.Collections;

// Unity
using UnityEngine;

// HLO
using HLO.Layer;
using HLO.Room;
using HLO.Item;

namespace HLO.Door
{
    public class LockedDoor : DoorBase
    {
        [SerializeField] private int necessaryKeyAmount = 1;
        [SerializeField] private Lock _lock;
        [SerializeField] private bool isRoomCleared;

        protected override void Awake()
        {
            base.Awake();

            Close();
        }

        protected override void RegisterRoomAction(RoomBase thisRoom)
        {
            thisRoom.RegisterOnClearRoom(() =>
            {
                isRoomCleared = true;
            });

            base.RegisterRoomAction(thisRoom);
        }

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == LayerDatas.PLAYER_LAYER && isRoomCleared)
            {
                if (IsOpen)
                {
                    connectedRoom.EnterRoom(DoorDirectionType, other.transform);
                }
                else
                {
                    Inventory inventory = other.gameObject.GetComponent<Inventory>();
                    while (necessaryKeyAmount-- > 0)
                    {
                        if (!inventory.UseItem(typeof(KeyItem)))
                        {
                            return;
                        }
                    }

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