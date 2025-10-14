
// Unity
using UnityEngine;

namespace HLO.Door
{
    public class OpenedDoor : DoorBase
    {
        protected override void Awake()
        {
            base.Awake();

            Open();
        }

        protected override void Open()
        {
            base.Open();

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if(spriteRenderer) spriteRenderer.enabled = false;
        }
    }
}