// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmAudioSource == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{gameObject.name} doesn't have {nameof(bgmAudioSource)}.");
#endif
            return;
        }

        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxAudioSource == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{gameObject.name} doesn't have {nameof(sfxAudioSource)}.");
#endif
            return;
        }

        sfxAudioSource.PlayOneShot(clip);
    }
}
