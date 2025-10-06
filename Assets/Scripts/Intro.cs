// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    [Header("Intro")]
    [SerializeField] private float introTime = 3f;

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

        SceneManager.LoadScene("Title");
    }
}
