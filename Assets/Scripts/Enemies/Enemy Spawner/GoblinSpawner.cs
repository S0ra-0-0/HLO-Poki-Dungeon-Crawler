// System
using System.Collections;
using System.Collections.Generic;
using System.Linq;


// Unity
using UnityEngine;

namespace HLO.Enemy.Spawn
{
    public class GoblinSpawner : EnemySpawner
    {
        public override  List<GameObject> Spawn()
        {
            List<Transform> spawnpointList = enemySpawnpointGroup.GetComponentsInChildren<Transform>().ToList();
            List<GameObject> enemyList = new List<GameObject>();
            int enemyAmount;

            for (enemyAmount = Random.Range(1, spawnpointList.Count); spawnpointList.Count > enemyAmount; spawnpointList.RemoveAt(Random.Range(0, spawnpointList.Count))) ;

            for (int i = 0; i < enemyAmount; i++)
            {
                enemyList.Add(Instantiate(enemyPrefabs[0], spawnpointList[i].position, Quaternion.identity));
            }

            return enemyList;
        }
    }
}