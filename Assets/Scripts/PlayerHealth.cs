// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Unity
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

// HLO
using HLO.Heart;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private List<HeartBase> heartList;
    [SerializeField] private Transform transformHPUI;
    [SerializeField] private GameObject prefabNormalHeart;
    [SerializeField] private int maxHP;
    [SerializeField] private int currentHP;
    private const int HP_PER_HEART = 2;
    private int NextHeartIndex => currentHP / HP_PER_HEART;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityTime;
    public bool isInvincibility; public bool IsInvincibility => isInvincibility;

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flickerNumber;

    [Header("Death")]
    [SerializeField] private string deathSceneName;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        heartList = transformHPUI.GetComponentsInChildren<HeartBase>().ToList();

        currentHP = maxHP = heartList.Count * HP_PER_HEART;
    }

    public void SetInvincibility(bool value) => isInvincibility = value;

    public void Hit(int damage)
    {
        if (IsInvincibility) { return; }

        DecreaseHP(damage);

        SetInvincibility(true);
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

        SetInvincibility(false);
    }

    public void AddHeart(GameObject objHeart)
    {
        Transform transformNewHeart = Instantiate(objHeart, objHeart.transform.position, Quaternion.identity).transform;

        transformNewHeart.SetParent(transformHPUI);
        heartList.Add(transformNewHeart.GetComponent<HeartBase>());

        maxHP += HP_PER_HEART;

        HealHP(maxHP - currentHP);
    }

    [ContextMenu("AddHeart")]
    public void TestAddHeart()
    {
        AddHeart(prefabNormalHeart);
    }

    [Header("Test Heal")] public int healTest;
    [ContextMenu("Heal")]
    public void TestHeal()
    {
        HealHP(healTest);
    }

    public void HealHP(int healAmount)
    {
        if (currentHP + healAmount > maxHP) healAmount = maxHP - currentHP;

        for (int i = NextHeartIndex; i < (currentHP + healAmount) / HP_PER_HEART; i++)
        {
            heartList[i].ChangeHeartShape(HP_PER_HEART);
        }

        currentHP += healAmount;

        if (currentHP % HP_PER_HEART != 0)
        {
            heartList[NextHeartIndex].ChangeHeartShape(1);
        }
    }

    private void DecreaseHP(int damage)
    {
        if (currentHP - damage < 0) damage = currentHP;

        for (int i = NextHeartIndex; i >= (currentHP - damage) / HP_PER_HEART; i--)
        {
            if (i >= heartList.Count) continue;
            heartList[i].ChangeHeartShape(0);
        }

        currentHP -= damage;

        if (currentHP % HP_PER_HEART != 0)
        {
            heartList[NextHeartIndex].ChangeHeartShape(1);
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        PokiUnitySDK.Instance.gameplayStop();
        SceneManager.LoadScene(deathSceneName);
    }
}
