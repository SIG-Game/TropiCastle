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

    public static PauseController Instance;

    private void Awake()
    {
        Instance = this;

        GamePaused = false;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
