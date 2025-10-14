using HLO.Door;
using HLO.Room;
using HLO.Layer;
using UnityEngine;

public class WallCrack : DoorBase
{
    public Sprite defaultWallSprite;
    public Sprite crackedWallSprite;
    public Sprite brokenWallSprite;

    private SpriteRenderer spriteRenderer;
    private enum WallState { Default, Cracked, Broken }
    private WallState currentState = WallState.Default;

    [SerializeField] private bool isRoomCleared = false;

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

    protected override void RegisterRoomAction(RoomBase thisRoom)
    {
        base.RegisterRoomAction(thisRoom);

        thisRoom.RegisterOnClearRoom(() =>
        {
            isRoomCleared = true;
        });
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerDatas.PLAYER_LAYER && isRoomCleared && IsOpen)
        {
            connectedRoom.EnterRoom(DoorDirectionType, other.transform);
        }
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
