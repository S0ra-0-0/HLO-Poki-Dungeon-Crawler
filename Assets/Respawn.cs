using UnityEngine;

public class Respawn : MonoBehaviour
{
    private void Start()
    {
        PokiUnitySDK.Instance.gameplayStart();
    }
}
