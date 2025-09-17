using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject wasdTutorialImage;
    [SerializeField] private GameObject attackTutorialImage;
    [SerializeField] private GameObject parryTutorialImage;

    private void Start()
    {
        InitializeTutorialUI();
    }

    private void Update()
    {
        HandleMovementInput();
        HandleAttackInput();
        HandleParryInput();
    }

    private void InitializeTutorialUI()
    {
        wasdTutorialImage.SetActive(true);
        attackTutorialImage.SetActive(false);
        parryTutorialImage.SetActive(false);
    }

    private void HandleMovementInput()
    {
        if (IsMovementKeyPressed() && wasdTutorialImage.activeSelf)
        {
            wasdTutorialImage.SetActive(false);
            attackTutorialImage.SetActive(true);
        }
    }

    private bool IsMovementKeyPressed()
    {
        return Input.GetKeyDown(KeyCode.W) ||
               Input.GetKeyDown(KeyCode.A) ||
               Input.GetKeyDown(KeyCode.S) ||
               Input.GetKeyDown(KeyCode.D);
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.J) && attackTutorialImage.activeSelf)
        {
            attackTutorialImage.SetActive(false);
            parryTutorialImage.SetActive(true);
        }
    }

    private void HandleParryInput()
    {
        if (Input.GetKeyDown(KeyCode.K) && parryTutorialImage.activeSelf)
        {
            parryTutorialImage.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
