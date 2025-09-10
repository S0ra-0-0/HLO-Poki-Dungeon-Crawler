// Unity
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class NormalHeart : Heart
{
    [SerializeField] private Image image = null;

    [SerializeField] private Sprite spriteFillHeart;
    [SerializeField] private Sprite spriteEmptyHeart;

    public override void Fill()
    {
        if (image == null) image = GetComponent<Image>();

        image.sprite = spriteFillHeart;
    }

    public override void Empty()
    {
        if (image == null) image = GetComponent<Image>();
        
        image.sprite = spriteEmptyHeart;
    }
}
