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

    // Call this method whenever a room is discovered
    public void OnRoomDiscovered()
    {
        DiscoveredRoomCount++;
        Debug.Log($"Room discovered! Total discovered rooms: {DiscoveredRoomCount}/{TotalRoomCount}");
        ProgressBar.fillAmount = (float)DiscoveredRoomCount / (TotalRoomCount/2);
    }
}
