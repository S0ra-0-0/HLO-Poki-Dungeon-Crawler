using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int monstersKilled = 0;
    public static GameManager instance;

    private const int baseDropRate = 20;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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

    public bool AttemptKeyDrop()
    {
        if (monstersKilled <= 0) return false;
        if (monstersKilled >= baseDropRate)
        {
          return true;
        }
        else { return false; }
    }

    public void KeyDrop(int keyDropRate)
    {
        monstersKilled += keyDropRate;
    }
}
