using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool gameHasStarted = false;
    private Player player;
    public TMPro.TextMeshProUGUI bossWeaponText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        PokiUnitySDK.Instance.init();
    }

    void Update()
    {
        HandleMovementInput();
    }


    public IEnumerator SpawnText()
    {
        bossWeaponText.color = Color.yellow;
        bossWeaponText.text = "Picked up Boss Club!";
        bossWeaponText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        bossWeaponText.gameObject.SetActive(false);
    }
    private void HandleMovementInput()
    {
        if (IsMovementKeyPressed() && !gameHasStarted)
        {
            PokiUnitySDK.Instance.gameplayStart();
            gameHasStarted = true;
        }
    }

    private bool IsMovementKeyPressed()
    {
        return Input.GetKeyDown(KeyCode.W) ||
               Input.GetKeyDown(KeyCode.A) ||
               Input.GetKeyDown(KeyCode.S) ||
               Input.GetKeyDown(KeyCode.D) ||
               Input.GetKeyDown(KeyCode.UpArrow) ||
               Input.GetKeyDown(KeyCode.LeftArrow) ||
               Input.GetKeyDown(KeyCode.DownArrow) ||
               Input.GetKeyDown(KeyCode.RightArrow);
    }


}
