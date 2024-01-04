using UnityEngine;

public class GameOverUIController : MonoBehaviour
{
    [SerializeField] private MenuProperties menuProperties;
    [SerializeField] private GameObject menuBackground;

    [Inject] private MenuManager menuManager;
    [Inject] private PlayerController playerController;

    private void Awake()
    {
        this.InjectDependencies();

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
