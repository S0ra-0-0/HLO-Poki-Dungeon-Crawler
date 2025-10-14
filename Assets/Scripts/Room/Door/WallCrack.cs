using HLO.Door;
using UnityEngine;

public class WallCrack : DoorBase
{
    public Sprite defaultWallSprite;
    public Sprite crackedWallSprite;
    public Sprite brokenWallSprite;

    private SpriteRenderer spriteRenderer;
    private enum WallState { Default, Cracked, Broken }
    private WallState currentState = WallState.Default;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("WallCrack: No SpriteRenderer found on the GameObject.");
            return;
        }
        spriteRenderer.sprite = defaultWallSprite;
    }

    public void crackWall()
    {
        if (currentState == WallState.Default)
        {
            if (crackedWallSprite != null)
            {
                spriteRenderer.sprite = crackedWallSprite;
                currentState = WallState.Cracked;
            }
            else
            {
                Debug.LogError("WallCrack: crackedWallSprite is null.");
            }
        }
        else if (currentState == WallState.Cracked)
        {
            if (brokenWallSprite != null)
            {
                spriteRenderer.sprite = brokenWallSprite;
                currentState = WallState.Broken;
                Open(); 
            }
            else
            {
                Debug.LogError("WallCrack: brokenWallSprite is null.");
            }
        }
    }
}
