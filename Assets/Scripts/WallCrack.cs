using UnityEngine;

public class WallCrack : MonoBehaviour
{
    public Sprite crackedWallSprite;
    public Sprite brokenWallSprite;
    private SpriteRenderer spriteRenderer;
    private bool isCracked = false;
    private void Start() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) 
        {
            Debug.LogError("WallCrack: No SpriteRenderer found on the GameObject.");
        }
    }
    public void crackWall() 
    {
        if (isCracked) return; // Prevent multiple cracks
        if (spriteRenderer != null && crackedWallSprite != null) 
        {
            spriteRenderer.sprite = brokenWallSprite;
            isCracked = true;
        } 
        else 
        {
            Debug.LogError("WallCrack: SpriteRenderer or crackedWallSprite is null.");
        }
    }
}
