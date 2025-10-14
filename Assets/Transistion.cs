using UnityEngine;
using System.Collections;

public class Transistion : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public IEnumerator rollTheSphere()
    {
        animator.SetTrigger("Transistion");
        yield return new WaitForSeconds(1f);
    }
}
