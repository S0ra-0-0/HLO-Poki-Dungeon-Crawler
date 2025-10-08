// System
using System;
using System.Collections;

// Unity
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Intro : MonoBehaviour
{
    [SerializeField] private float introTime = 3f;

    [SerializeField] private SceneAsset sceneAsset;

    private void Start()
    {
        StartCoroutine(IntroCoroutine());
    }

    private IEnumerator IntroCoroutine()
    {
        float timer = 0f;
        while (timer <= introTime && Application.isPlaying)
        {
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.KeypadEnter))
            {
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(sceneAsset.name);
    }
}
