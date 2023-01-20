using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private PlayerController playerController;

    private void Update()
    {
        if (Input.GetButtonDown("Pause") &&
            (!PauseController.Instance.PauseMenuOpen || pauseMenuUI.activeInHierarchy))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        PauseController.Instance.GamePaused = !PauseController.Instance.GamePaused;
        PauseController.Instance.PauseMenuOpen = PauseController.Instance.GamePaused;
        pauseMenuUI.SetActive(PauseController.Instance.PauseMenuOpen);
        playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
