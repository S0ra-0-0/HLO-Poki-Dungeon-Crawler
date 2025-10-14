// Unity
using UnityEngine;
using UnityEngine.SceneManagement;

// HLO
using HLO.Layer;

public class ToNextLevelPortal : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private Transistion transistion;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerDatas.PLAYER_LAYER)
        {
            transistion.StartCoroutine(transistion.rollTheSphere());
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
