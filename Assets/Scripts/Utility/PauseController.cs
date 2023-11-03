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

            OnGamePausedSet();
        }
    }

    public event Action OnGamePausedSet = () => {};

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
