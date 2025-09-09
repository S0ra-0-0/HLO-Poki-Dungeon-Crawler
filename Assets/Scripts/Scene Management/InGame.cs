// Unity
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// SceneManagement
using SceneManagement;

public class InGame : MonoBehaviour
{
    [Header("Temp Death")]
    [SerializeField] private Button btnDeath;
    [SerializeField] private SceneAsset sceneDeath;

    private void Start()
    {
        btnDeath.onClick.AddListener(() => SceneLoader.SceneLoad(sceneDeath.name));
    }
}
