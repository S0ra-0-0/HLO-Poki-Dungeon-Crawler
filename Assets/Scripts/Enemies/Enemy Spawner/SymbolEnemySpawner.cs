// System
using System.Linq;
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
                enemyList.Add(Instantiate(enemyPrefabs[0], spawnpointList[i].position, Quaternion.identity));
            }

            return enemyList;
        }
    }
}