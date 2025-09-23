// Unity
using UnityEngine;

public class Lock : MonoBehaviour
{
    [SerializeField] private Sprite unlockSprite;

    public void Unlock()
    {
        GetComponent<SpriteRenderer>().sprite = unlockSprite;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
