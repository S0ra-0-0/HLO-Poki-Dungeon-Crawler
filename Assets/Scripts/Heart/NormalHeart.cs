// Unity
using System;
using UnityEngine;
using UnityEngine.UI;

namespace HLO.Heart
{
    [RequireComponent(typeof(Image))]
    public class NormalHeart : HeartBase
    {
        [SerializeField] private Image image = null;

        [SerializeField] private Sprite[] heartSprites;

        public override void ChangeHeartShape(int index)
        {
            if (!image) image = GetComponent<Image>();

            image.sprite = heartSprites[index];
        }
    }
}