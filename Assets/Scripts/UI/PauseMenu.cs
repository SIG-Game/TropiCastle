using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private MenuProperties menuProperties;
    [SerializeField] private GameObject menuBackground;
    [SerializeField] private MenuManager menuManager;
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
            (!PauseController.Instance.GamePaused ||
                menuProperties.MenuIsVisible))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        PauseController.Instance.GamePaused = !PauseController.Instance.GamePaused;

        if (PauseController.Instance.GamePaused)
        {
            menuManager.ShowMenu(menuProperties);
        }
        else
        {
            menuManager.HideCurrentMenu();
        }

        menuBackground.SetActive(PauseController.Instance.GamePaused);

        playerRigidbody2D.velocity = Vector2.zero;
    }
}
