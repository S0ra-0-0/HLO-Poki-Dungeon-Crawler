using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int roomAmount = int.MaxValue; public int RoomAmount => roomAmount;
    [SerializeField] private int clearedRoomCount = 0; public int ClearedRoomCount => clearedRoomCount;
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        PokiUnitySDK.Instance.init();
    }
    
    public void SetRoomAmount(int amount) => roomAmount = amount;
    public void RoomClear()
    {
        clearedRoomCount++;
        if (clearedRoomCount >= roomAmount)
        {
            PokiUnitySDK.Instance.gameplayStop();
            SceneManager.LoadScene("Clear Scene");
        }
    }
}
