using UnityEngine;

public class GameOverUIController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject menuBackground;
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
        gameOverUI.SetActive(true);
        menuBackground.SetActive(true);

        EventSystemDefaultGameObjectSelector.Instance
            .SetDefaultSelectedGameObject(reloadButton);
    }
}
