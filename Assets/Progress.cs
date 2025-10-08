using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    public static Progress Instance { get; private set; }

    public Image ProgressBar;
    [SerializeField] private TMP_Text progressText;
    public int TotalRoomCount { get; private set; }
    public int EnteredRoomCount { get; private set; }

    private Action onProgressReward;

    private bool bossRoomHasBeenFound = false;
    public void RegisterOnProgressReward(Action action) => onProgressReward += action;
    public void UnregisterOnProgressReward(Action action) => onProgressReward -= action;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            TotalRoomCount = int.MaxValue;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeTotalRoomCount(int totalRoomCount)
    {
        ProgressBar.fillAmount = 0f;
        progressText.gameObject.SetActive(false);

        TotalRoomCount = totalRoomCount;
        EnteredRoomCount = 0;
        Debug.Log($"Total rooms initialized: {TotalRoomCount}");
    }

    public void OnProgressUpdated()
    {
        EnteredRoomCount++;
        Debug.Log($"Room discovered! Total discovered rooms: {EnteredRoomCount}/{TotalRoomCount}");
        ProgressBar.fillAmount = (float)EnteredRoomCount / (TotalRoomCount / 2);

        if (ProgressBar.fillAmount >= 1f)
        {
            Debug.Log("Progress bar is full!");
            StartCoroutine(SpawnText());
            ProgressReward();

        }
    }

    IEnumerator SpawnText()
    {
        if (bossRoomHasBeenFound) yield break;
        progressText.gameObject.SetActive(true);
        bossRoomHasBeenFound = true;
        yield return new WaitForSeconds(2f);
        progressText.gameObject.SetActive(false);
    }

    private void ProgressReward()
    {
        Debug.Log("Progress bar is full! Rewarding player...");
        // Implement reward logic here
         Player.FindFirstObjectByType<Player>().bossKeyFound = true;
        onProgressReward?.Invoke();
        onProgressReward = null;
    }
}
