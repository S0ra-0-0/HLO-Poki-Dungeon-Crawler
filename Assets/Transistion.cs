using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Transistion : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public IEnumerator rollTheSphere(string sceneToLoad)
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
