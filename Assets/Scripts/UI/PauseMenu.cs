using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InputAction pauseInputAction;
    [SerializeField] private GameObject resumeButton;

    private void Start()
    {
        pauseInputAction = InputManager.Instance.GetAction("Pause");
    }

    private void Update()
    {
        if (InputManager.Instance.EscapeKeyUsedThisFrame)
        {
            return;
        }

        if (pauseInputAction.WasPressedThisFrame() &&
            (!PauseController.Instance.GamePaused || pauseMenuUI.activeInHierarchy))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        PauseController.Instance.GamePaused = !PauseController.Instance.GamePaused;
        pauseMenuUI.SetActive(PauseController.Instance.GamePaused);
        playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        if (PauseController.Instance.GamePaused)
        {
            EventSystemDefaultGameObjectSelector.Instance
                .SetDefaultSelectedGameObject(resumeButton);
        }
        else
        {
            EventSystemDefaultGameObjectSelector.Instance
                .SetDefaultSelectedGameObject(null);

            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
