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
        ProgressBar.fillAmount = (float)EnteredRoomCount / (TotalRoomCount / 2); // Why are you dividing by 2?

        if (ProgressBar.fillAmount >= .1f)
        {
            Debug.Log("Progress bar is full!");
            StartCoroutine(SpawnText());
            ProgressReward();

        }
    }

    IEnumerator SpawnText()
    {
        progressText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        progressText.gameObject.SetActive(false);
    }

    private void ProgressReward()
    {
        Debug.Log("Progress bar is full! Rewarding player...");
        // Implement reward logic here
    }
}
