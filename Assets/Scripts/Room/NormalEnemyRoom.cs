// System
using System;
using System.Collections;
using System.Collections.Generic;
// HLO
using HLO.Enemy.Spawn;
// Unity
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HLO.Room
{
    public class NormalEnemyRoom : RoomBase
    {
        [SerializeField] EnemySpawner enemySpawner;
        [SerializeField] bool enemySpawned;
        [SerializeField] List<GameObject> remainedEnemyList;

        private void Awake()
        {
            enemySpawner = GetComponent<EnemySpawner>();

            if (enemySpawner)
            {
                RegisterOnEnterRoom(SpawnEnemy);
                RegisterOnClearRoom(() =>
                {
                    UnregisterOnEnterRoom(SpawnEnemy);
                });
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} doesn't have {nameof(EnemySpawner)}.");
            }

            RegisterOnClearRoom(() =>
            {
                if (SceneManager.GetActiveScene().name == "Level 01") { PlayerHealth.FindAnyObjectByType<PlayerHealth>().HealHP(5); }
            });
        }

        private void SpawnEnemy()
        {
            enemySpawned = true;
            remainedEnemyList = enemySpawner.Spawn();
        }

        private void Update()
        {
            if (!enemySpawned) return;

            if (remainedEnemyList.Count > 0) // TODO: It's very very bad. We should fix it. 
                                             // I want to register an action that is invoked when an enemy class dies.
            {
                for (int i = 0; i < remainedEnemyList.Count; i++)
                {
                    if (!remainedEnemyList[i])
                    {
                        remainedEnemyList.RemoveAt(i--);
                    }
                }
            }
            else
            {
                enemySpawned = false;
                ClearRoom();
            }
        }
    }
}