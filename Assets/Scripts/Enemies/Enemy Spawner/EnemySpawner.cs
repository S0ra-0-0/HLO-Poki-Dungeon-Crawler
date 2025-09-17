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
        [SerializeField] protected Transform enemySpawnpointGroup;
        [SerializeField] protected GameObject[] enemyPrefabs;

        public abstract  List<GameObject> Spawn();
    }
}