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

    private const float ITEM_TO_PLAYER_TIME = 0.75f;

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

        StartCoroutine(OpenCoroutine(player));
    }

    private IEnumerator OpenCoroutine(Transform player)
    {
        _lock.Unlock();

        yield return new WaitForSeconds(0.25f);

        _lock.Hide();

        StartCoroutine(DropItemsCoroutine(player));
    }

    private IEnumerator DropItemsCoroutine(Transform target)
    {
        Vector2 StartPos = transform.position;

        Transform[] itemTransforms = new Transform[items.Length];
        Vector2[] middlePositions = new Vector2[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            itemTransforms[i] = Instantiate(items[i].gameObject, transform.position, Quaternion.identity).transform;

            middlePositions[i] = StartPos + Random.insideUnitCircle * Random.Range(MIN_MIDDLE_POS_OFFSET, MAX_MIDDLE_POS_OFFSET);
        }
        
        float t = 0f;
        while (t <= ITEM_TO_PLAYER_TIME)
        {
            t += Time.deltaTime;

            for (int i = 0; i < items.Length; i++)
            {
                if (!itemTransforms[i]) continue;

                itemTransforms[i].position = BezierCurve(StartPos, middlePositions[i], target.position, t / ITEM_TO_PLAYER_TIME);
            }

            yield return null;
        }
    }
    
    Vector2 BezierCurve(Vector2 start, Vector2 middle, Vector2 end, float t)
    {
        return  (1 - t) * (1 - t) * start
                + 2 * (1 - t) * t * middle
                + t * t * end;
    }
}
