using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject menuBackground;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private InputActionReference pauseActionReference;
    [SerializeField] private EventSystemDefaultGameObjectSelector eventSystemDefaultGameObjectSelector;

    private InputAction pauseAction;

    private void Awake()
    {
        pauseAction = pauseActionReference.action;
    }

    private void Update()
    {
        if (pauseAction.WasPressedThisFrame() &&
            (!PauseController.Instance.GamePaused || pauseMenuUI.activeInHierarchy))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        PauseController.Instance.GamePaused = !PauseController.Instance.GamePaused;
        pauseMenuUI.SetActive(PauseController.Instance.GamePaused);
        menuBackground.SetActive(PauseController.Instance.GamePaused);
        playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        if (PauseController.Instance.GamePaused)
        {
            eventSystemDefaultGameObjectSelector
                .SetDefaultSelectedGameObject(resumeButton);
        }
        else
        {
            eventSystemDefaultGameObjectSelector
                .SetDefaultSelectedGameObject(null);

            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
