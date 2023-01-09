using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerController playerController;

    private bool gamePaused;

    public bool CanPause { get; set; }
    public bool GamePaused
    {
        get => gamePaused;
        set
        {
            gamePaused = value;
            Time.timeScale = gamePaused ? 0f : 1f;
        }
    }

    public static PauseController Instance;

    private void Awake()
    {
        Instance = this;

        CanPause = true;
        GamePaused = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause") && !playerController.GetInventoryOpen() && CanPause)
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
        pauseMenu.SetActive(GamePaused);
        playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
