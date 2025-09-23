// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;



// Unity
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private List<Heart> heartList;
    [SerializeField] private Transform transformHPUI;
    [SerializeField] private GameObject prefabNormalHeart;
    [SerializeField] private int maxHP;
    [SerializeField] private int currentHP;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityTime;
    public bool isInvincibility; public bool IsInvincibility => isInvincibility;

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flickerNumber;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        heartList = transformHPUI.GetComponentsInChildren<Heart>().ToList();

        currentHP = maxHP = heartList.Count;
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
        heartList.Add(transformNewHeart.GetComponent<Heart>());

        HealHP(++maxHP - currentHP);
    }

    [ContextMenu("AddHeart")]
    public void TestAddHeart()
    {
        AddHeart(prefabNormalHeart);
    }

    [ContextMenu("Heal")]
    public void TestHeal()
    {
        HealHP(1);
    }

    private void HealHP(int healedAmount)
    {
        while (healedAmount-- > 0)
        {
            if (currentHP >= maxHP)
            {
                break;
            }

            heartList[currentHP++].Fill();
        }
    }

    private void DecreaseHP(int damage)
    {
        while (damage-- > 0)
        {
            if (--currentHP <= 0)
            {
                Die();
                break;
            }

            heartList[currentHP].Empty();
        }
    }
    private void Die()
    {
        GameManager.FindAnyObjectByType<GameManager>().monstersKilled = 0;
        PokiUnitySDK.Instance.gameplayStop();
        SceneManager.LoadScene("DeathScene");
    }
}
