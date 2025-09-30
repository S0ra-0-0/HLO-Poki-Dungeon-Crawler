// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace HLO.Room
{
    public class TutorialRoom : RoomBase
    {
        [SerializeField] List<GameObject> tutorialEnemies = new List<GameObject>();

        protected override void Start()
        {
            base.Start();
            
            Discover();
            onEnterRoom?.Invoke();
            SetRoomType(RoomType.Tutorial);
        }

        private void Update()
        {
            // TODO: It's very very bad. We should fix it. 
            // I want to register an action that is invoked when an enemy class dies.
            if (tutorialEnemies.Count == 0) return;

            for (int i = 0; i < tutorialEnemies.Count; i++)
            {
                if (tutorialEnemies[i] == null)
                {
                    tutorialEnemies.RemoveAt(i--);

                    if (tutorialEnemies.Count == 0)
                    {
                        ClearRoom();
                    }
                } 
            }
        }
    }
}