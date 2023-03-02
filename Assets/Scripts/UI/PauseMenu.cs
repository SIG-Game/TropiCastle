using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private PlayerController playerController;

    private void Update()
    {
        if (InputManager.Instance.EscapeKeyUsedThisFrame)
        {
            return;
        }

        if (Input.GetButtonDown("Pause") &&
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
    }
}
