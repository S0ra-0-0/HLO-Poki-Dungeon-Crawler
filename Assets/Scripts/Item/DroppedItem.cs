// Unity
using UnityEngine;
using UnityEngine.EventSystems;

// HLO
using HLO.Layer;

namespace HLO.Item
{
    public class DroppedItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] protected ItemBase item;
        [SerializeField] protected Vector2 tooltipPositionOffset;

        protected virtual void Awake()
        {
            item = GetComponent<ItemBase>();

            if (!item) Debug.LogError($"{gameObject.name} Doesn't have Item!");
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerDatas.PLAYER_LAYER)
            {
                item.Get(other.gameObject);

                Destroy(gameObject);
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            Tooltip.Instance.Rent((Vector2)transform.position + tooltipPositionOffset, item.Name, item.Description);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            Tooltip.Instance.Return();
        }
    }
}