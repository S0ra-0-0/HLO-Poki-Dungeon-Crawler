// Unity
using UnityEngine;

public class Lock : MonoBehaviour
{
    [SerializeField] private Sprite unlockSprite;

    public void Unlock(/*float keepingTime*/)
    {
        GetComponent<SpriteRenderer>().sprite = unlockSprite;
        // Invoke(nameof(Hide), keepingTime);
    }

    // private void Hide()
    // {
    //     gameObject.SetActive(false);
    // }
}
