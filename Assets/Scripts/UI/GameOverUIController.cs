using UnityEngine;

public class GameOverUIController : MonoBehaviour
{
    [SerializeField] private MenuProperties menuProperties;
    [SerializeField] private GameObject menuBackground;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private PlayerController playerController;

    private void Awake()
    {
        playerController.OnPlayerDied += PlayerController_OnPlayerDied;
    }

    private void OnDestroy()
    {
        playerController.OnPlayerDied -= PlayerController_OnPlayerDied;
    }

    private void PlayerController_OnPlayerDied()
    {
        menuBackground.SetActive(true);

        menuManager.ShowMenu(menuProperties);
    }
}
