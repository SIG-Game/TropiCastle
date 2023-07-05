using UnityEngine;

public class GameOverUIController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUIToActivateOnPlayerDeath;
    [SerializeField] private GameObject reloadButton;
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
        gameOverUIToActivateOnPlayerDeath.SetActive(true);

        EventSystemDefaultGameObjectSelector.Instance
            .SetDefaultSelectedGameObject(reloadButton);
    }
}
