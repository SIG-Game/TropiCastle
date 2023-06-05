using UnityEngine;

public class DebugModeController : MonoBehaviour
{
    [SerializeField] private bool debugModeEnabled;

    public static bool DebugModeEnabled;

    private void Awake()
    {
        DebugModeEnabled = debugModeEnabled;
    }
}
