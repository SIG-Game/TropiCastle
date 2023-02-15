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

            if (gamePaused)
            {
                OnGamePaused();
            }
            else
            {
                OnGameUnpaused();
            }
        }
    }

    public static PauseController Instance;

    public static event Action OnGamePaused;
    public static event Action OnGameUnpaused;

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
