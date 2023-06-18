using System;
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

            OnGamePausedSet(gamePaused);
        }
    }

    public static PauseController Instance;

    public static event Action<bool> OnGamePausedSet = delegate { };

    private void Awake()
    {
        Instance = this;

        GamePaused = false;
    }

    private void OnDestroy()
    {
        Instance = null;

        OnGamePausedSet = delegate { };
    }
}
