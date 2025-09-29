// Unity
using UnityEngine;
using UnityEngine.EventSystems;

// TMP
using TMPro;

// HLO
using HLO.Layer;

namespace HLO.Item
{
    public abstract class DisplayedItem : MonoBehaviour, IItem, IPointerDownHandler, IPointerUpHandler
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [SerializeField] TMP_Text priceText;
        [SerializeField] protected int price;
        
        [Space(1)] [SerializeField] protected Vector2 tooltipPositionOffset;

        public abstract void Use(GameObject player);

        public void SetItem(string name, string description, int price)
        {
            Name = name;
            Description = description;
            this.price = price;

            priceText.text = price.ToString("D2");
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerDatas.PLAYER_LAYER && other.GetComponent<Inventory>().UseCoins(price))
            {
                Use(other.gameObject);

                GetComponent<Collider2D>().enabled = false;
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Tooltip.Instance.Rent((Vector2)transform.position + tooltipPositionOffset, Name, Description);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Tooltip.Instance.Return();
        }
    }
}