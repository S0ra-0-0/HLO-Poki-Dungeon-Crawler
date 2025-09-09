using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public static class SceneLoader
    {
        public static void SceneLoad(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(sceneName, mode);
        }
    }
}