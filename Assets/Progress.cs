using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    public static Progress Instance { get; private set; }

    public Image ProgressBar;
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
        TotalRoomCount = totalRoomCount;
        EnteredRoomCount = 0;
        Debug.Log($"Total rooms initialized: {TotalRoomCount}");
    }

    public void OnProgressUpdated()
    {
        EnteredRoomCount++;
        Debug.Log($"Room discovered! Total discovered rooms: {EnteredRoomCount}/{TotalRoomCount}");
        ProgressBar.fillAmount = (float)EnteredRoomCount / TotalRoomCount;

        if (ProgressBar.fillAmount == 1f)
        {
            ProgressReward();
        }
    }

    private void ProgressReward()
    {
        Debug.Log("Progress bar is full! Rewarding player...");
        // Implement reward logic here
    }
}
