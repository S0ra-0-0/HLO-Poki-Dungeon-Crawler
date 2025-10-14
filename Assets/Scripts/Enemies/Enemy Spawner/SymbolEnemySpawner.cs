// System
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace HLO.Enemy.Spawn
{
    public class SymbolEnemySpawner : EnemySpawner
    {
        public override List<GameObject> Spawn()
        {
            List<Transform> spawnpointList = GetSpawnpointList();
            List<GameObject> enemyList = new List<GameObject>();

            for (int i = 0; i < spawnpointList.Count; i++)
            {
                enemyList.Add(Instantiate(enemyPrefabs[i < enemyPrefabs.Length ? i : Random.Range(0, enemyPrefabs.Length)], spawnpointList[i].position, Quaternion.identity));
            }

            return enemyList;
        }
    }
}