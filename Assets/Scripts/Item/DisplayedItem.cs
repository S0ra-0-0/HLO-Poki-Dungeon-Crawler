// Unity
using UnityEngine;

// TMP
using TMPro;

// HLO
using HLO.Layer;

namespace HLO.Item
{
    public class DisplayedItem : DroppedItem
    {
        [SerializeField] protected TMP_Text priceText;
        [SerializeField] protected int price;

        public void SetPrice(int price)
        {
            this.price = price;

            priceText.text = price.ToString("D2");
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerDatas.PLAYER_LAYER && other.GetComponent<Inventory>().UseCoins(price))
            {
                item.Use(other.gameObject);

                GetComponent<Collider2D>().enabled = false;
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        public void Discard()
        {
            Destroy(gameObject);
        }
    }
}