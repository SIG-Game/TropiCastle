using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private MenuProperties menuProperties;
    [SerializeField] private GameObject menuBackground;
    [SerializeField] private Rigidbody2D playerRigidbody2D;
    [SerializeField] private InputActionReference pauseActionReference;

    [Inject] private MenuManager menuManager;
    [Inject] private PauseController pauseController;

    private InputAction pauseAction;

    private void Awake()
    {
        this.InjectDependencies();

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
