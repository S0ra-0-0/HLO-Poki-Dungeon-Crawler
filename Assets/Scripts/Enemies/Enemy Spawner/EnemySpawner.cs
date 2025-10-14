// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace HLO.Enemy.Spawn
{
    public abstract class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform enemySpawnpointGroup;
        [SerializeField] protected GameObject[] enemyPrefabs;

        public abstract List<GameObject> Spawn();

        protected virtual List<Transform> GetSpawnpointList()
        {
            List<Transform> spawnpointList = new List<Transform>();

            for (int i = 0; i < enemySpawnpointGroup.childCount; i++)
            {
                spawnpointList.Add(enemySpawnpointGroup.GetChild(i));
            }

            return spawnpointList;
        }
    }
}