using UnityEngine;

public class GameOverUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup gameOverUICanvasGroup;
    [SerializeField] private GameObject menuBackground;
    [SerializeField] private GameObject reloadButton;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EventSystemDefaultGameObjectSelector eventSystemDefaultGameObjectSelector;

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
        gameOverUICanvasGroup.ShowAndMakeInteractable();

        menuBackground.SetActive(true);

        eventSystemDefaultGameObjectSelector
            .SetDefaultSelectedGameObject(reloadButton);
    }
}
