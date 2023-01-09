using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerController playerController;

    private bool gamePaused;

    public bool CanOpenPauseMenu { get; set; }
    public bool GamePaused
    {
        get => gamePaused;
        set
        {
            gamePaused = value;
            Time.timeScale = gamePaused ? 0f : 1f;
        }
    }
    public bool PauseMenuOpen { get; private set; }

    public static PauseController Instance;

    private void Awake()
    {
        Instance = this;

        CanOpenPauseMenu = true;
        GamePaused = false;
        PauseMenuOpen = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause") && CanOpenPauseMenu)
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
        GamePaused = !GamePaused;
        PauseMenuOpen = GamePaused;
        pauseMenu.SetActive(PauseMenuOpen);
        playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
