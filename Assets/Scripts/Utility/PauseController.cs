using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerController playerController;

    public bool canPause { get; set; }
    public bool gamePaused { get; set; }

    public static PauseController Instance;

    private void Awake()
    {
        Instance = this;

        canPause = true;
        gamePaused = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause") && !playerController.GetInventoryOpen() && canPause)
        {
            TogglePauseMenu();
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void TogglePauseMenu()
    {
        gamePaused = !gamePaused;
        pauseMenu.SetActive(gamePaused);
        playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Time.timeScale = gamePaused ? 0f : 1f;
    }
}
