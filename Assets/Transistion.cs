using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Transistion : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float transitionTime = 1f;
    [SerializeField] private Player player;

    private void OnEnable()
    {
      player.gameObject.SetActive(false);
      animator.SetTrigger("Start");
      StartCoroutine(EnablePlayerAfterTransition());

    }
    private IEnumerator EnablePlayerAfterTransition()
    {
        yield return new WaitForSeconds(transitionTime);
        player.gameObject.SetActive(true);
    }
}
