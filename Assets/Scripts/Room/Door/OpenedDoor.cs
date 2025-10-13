
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

            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}