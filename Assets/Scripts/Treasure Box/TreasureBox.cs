// System
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

//HLO
using HLO.Item;
using HLO.Layer;

public class TreasureBox : MonoBehaviour
{
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private Lock _lock;
    [SerializeField] private ItemBase[] items;
    [SerializeField] private bool isOpened = false;
    [SerializeField] private int necessaryKeyAmount = 1;

    private const float MIN_MIDDLE_POS_OFFSET = 1f;
    private const float MAX_MIDDLE_POS_OFFSET = 4.5f;

    private const float ITEM_TO_PLAYER_TIME = 0.25f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerDatas.PLAYER_LAYER && !isOpened)
        {
            Inventory inventory = collision.gameObject.GetComponent<Inventory>();
            while (necessaryKeyAmount-- > 0)
            {
                if (!inventory.UseItem(typeof(KeyItem)))
                {
                    return;
                }
            }

            Open(collision.transform);
        }
    }

    private void Open(Transform player)
    {
        GetComponent<SpriteRenderer>().sprite = openedSprite;
        isOpened = true;

        _lock.Unlock();

        StartCoroutine(DropItemsCoroutine(player));
    }

    private IEnumerator DropItemsCoroutine(Transform target)
    {
        foreach (ItemBase item in items)
        {
            Transform itemTransform = Instantiate(item.gameObject, transform.position, Quaternion.identity).transform;
            Vector2 StartPos = transform.position;
            Vector2 middlePos = StartPos + Random.insideUnitCircle * Random.Range(MIN_MIDDLE_POS_OFFSET, MAX_MIDDLE_POS_OFFSET);

            float t = 0f;
            while (t <= ITEM_TO_PLAYER_TIME)
            {
                if (!itemTransform) break;

                itemTransform.position = BezierCurve(StartPos, middlePos, target.position, t / ITEM_TO_PLAYER_TIME);
                t += Time.deltaTime;

                yield return null;
            }
        }
    }
    
    Vector2 BezierCurve(Vector2 start, Vector2 middle, Vector2 end, float t)
    {
        return  (1 - t) * (1 - t) * start
                + 2 * (1 - t) * t * middle
                + t * t * end;
    }
}
