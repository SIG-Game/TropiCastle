using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private MenuProperties menuProperties;
    [SerializeField] private GameObject menuBackground;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private PauseController pauseController;
    [SerializeField] private Rigidbody2D playerRigidbody2D;
    [SerializeField] private InputActionReference pauseActionReference;

    private InputAction pauseAction;

    private void Awake()
    {
        pauseAction = pauseActionReference.action;
    }

    private void Update()
    {
        if (pauseAction.WasPressedThisFrame() &&
            (!pauseController.GamePaused ||
                menuProperties.MenuIsVisible))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        pauseController.GamePaused = !pauseController.GamePaused;

        if (pauseController.GamePaused)
        {
            menuManager.ShowMenu(menuProperties);
        }
        else
        {
            menuManager.HideCurrentMenu();
        }

        menuBackground.SetActive(pauseController.GamePaused);

        playerRigidbody2D.velocity = Vector2.zero;
    }
}
