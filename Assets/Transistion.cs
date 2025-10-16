using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Transistion : MonoBehaviour
{
    [SerializeField] private Animator floorAnimator;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float transitionTime = 1f;
    [SerializeField] private Player player;
    [SerializeField] private GameObject transitionCanvasObject;

    private void OnEnable()
    {
      player.gameObject.SetActive(false);
      floorAnimator.SetTrigger("Start");
      StartCoroutine(PlayerAnimation());
    }

    private IEnumerator PlayerAnimation()
    {
        yield return new WaitForSeconds(1f);
        playerAnimator.SetTrigger("Start");
        StartCoroutine(EnablePlayerAfterTransition());
    }
    private IEnumerator EnablePlayerAfterTransition()
    {
        yield return new WaitForSeconds(transitionTime);
        player.gameObject.SetActive(true);
        transitionCanvasObject.SetActive(false);
    }
}
