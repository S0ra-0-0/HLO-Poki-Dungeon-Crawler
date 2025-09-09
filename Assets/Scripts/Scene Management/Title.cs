// Unity
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// SceneManagement
using SceneManagement;

public class Title : MonoBehaviour
{
    [Header("Start Game")]
    [SerializeField] private Button btnStart;
    [SerializeField] private SceneAsset sceneInGame;

    [Header("Quit Game")]
    [SerializeField] private Button bntQuit;

    private void Start()
    {
        btnStart.onClick.AddListener(() => SceneLoader.SceneLoad(sceneInGame.name));
        bntQuit.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        });
    }

    private void OnDestroy()
    {
        btnStart.onClick.RemoveAllListeners();
        bntQuit.onClick.RemoveAllListeners();
    }
}
