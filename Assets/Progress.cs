using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{

    public Image ProgressBar;
    public int TotalRoomCount { get; private set; }
    public int DiscoveredRoomCount { get; private set; }

  
    public void InitializeTotalRoomCount(int totalRoomCount)
    {
        ProgressBar.fillAmount = 0f;
        TotalRoomCount = totalRoomCount;
        DiscoveredRoomCount = 0;
        Debug.Log($"Total rooms initialized: {TotalRoomCount}");
    }
    public void OnRoomDiscovered()
    {
        DiscoveredRoomCount++;
        Debug.Log($"Room discovered! Total discovered rooms: {DiscoveredRoomCount}/{TotalRoomCount}");
        ProgressBar.fillAmount = (float)DiscoveredRoomCount / (TotalRoomCount/2);

        if (ProgressBar.fillAmount == 100f)
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
