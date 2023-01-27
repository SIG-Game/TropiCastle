using UnityEngine;

public class GameOverUIController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUIToActivateOnPlayerDeath;

    private void Awake()
    {
        PlayerController.OnPlayerDied += PlayerController_OnPlayerDied;
    }

    private void OnDestroy()
    {
        PlayerController.OnPlayerDied -= PlayerController_OnPlayerDied;
    }

    private void PlayerController_OnPlayerDied()
    {
        gameOverUIToActivateOnPlayerDeath.SetActive(true);
    }
}
