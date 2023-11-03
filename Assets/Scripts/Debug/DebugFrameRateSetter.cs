using UnityEngine;

public class DebugFrameRateSetter : MonoBehaviour
{
    [SerializeField] private int targetFrameRate;

    private void Update()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
