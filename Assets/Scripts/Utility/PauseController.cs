using UnityEngine;

public class PauseController : MonoBehaviour
{
    private bool gamePaused;

    public bool GamePaused
    {
        get => gamePaused;
        set
        {
            gamePaused = value;
            Time.timeScale = gamePaused ? 0f : 1f;
        }
    }
    public bool PauseMenuOpen { get; set; }

    public static PauseController Instance;

    private void Awake()
    {
        Instance = this;

        GamePaused = false;
        PauseMenuOpen = false;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
