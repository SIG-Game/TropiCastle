using UnityEngine;

public class DebugController : MonoBehaviour
{
    [SerializeField] private int targetFrameRate;
    [SerializeField] private bool debugModeEnabled;

    public static bool DebugModeEnabled;

    private void Awake()
    {
        DebugModeEnabled = debugModeEnabled;
    }

    private void Update()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
