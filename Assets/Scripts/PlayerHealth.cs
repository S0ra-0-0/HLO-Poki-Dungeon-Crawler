// System
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private List<Heart> heartList;
    [SerializeField] private int maxHP;
    [SerializeField] private int currentHP;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityTime;
    [SerializeField] private bool isInvincibility;

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flickerNumber;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHP = maxHP = heartList.Count;
    }

    public void Hit(int damage)
    {
        if (isInvincibility) { return; }

        DecreaseHP(damage);

        isInvincibility = true;
        StartCoroutine(CoroutineSignInvincibility());
    }

    private IEnumerator CoroutineSignInvincibility()
    {
        float timer = 0f;

        Color color = spriteRenderer.color;

        float flickerOffset = flickerNumber * 2f * Mathf.PI / invincibilityTime;

        while (timer < invincibilityTime)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Sin(timer * flickerOffset) * 0.5f + 0.5f;

            spriteRenderer.color = color;

            yield return null;
        }

        color.a = 1f;
        spriteRenderer.color = color;

        isInvincibility = false;
    }

    private void DecreaseHP(int damage)
    {
        
    }
}
