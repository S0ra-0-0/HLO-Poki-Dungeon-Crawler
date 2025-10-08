// System
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

// Unity
using UnityEngine;
using UnityEngine.SceneManagement;

// HLO
using HLO.Layer;

public class ToNextLevelPortal : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerDatas.PLAYER_LAYER)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
