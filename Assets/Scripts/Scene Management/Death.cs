// Unity
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// SceneManagement
using SceneManagement;

public class Death : MonoBehaviour
{
    [Header("To Title")]
    [SerializeField] private Button bntToTitle;
    [SerializeField] private SceneAsset sceneTitle;

    private void Start()
    {
        bntToTitle.onClick.AddListener(() => SceneLoader.SceneLoad(sceneTitle.name));
    }
}
