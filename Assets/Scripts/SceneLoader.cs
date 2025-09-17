using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad = "Your scene name here"; 
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
        Debug.Log($"Loading scene: {sceneToLoad}");
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed.");
#if UNITY_EDITOR
        Debug.Log("Exiting Play Mode in Editor...");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.Log("Quitting application...");
        Application.Quit();
#endif
    }
}
