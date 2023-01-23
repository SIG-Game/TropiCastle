using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private bool leftClickInputUsedThisFrame;

    private void Awake()
    {
        Instance = this;
    }

    // Has to run before any scripts that use GetLeftClickDownIfUnusedThisFrame method
    private void Update()
    {
        leftClickInputUsedThisFrame = false;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    // Returns left-click down bool when that input has not been used this frame
    // Otherwise, returns false
    public bool GetLeftClickDownIfUnusedThisFrame()
    {
        if (!leftClickInputUsedThisFrame)
        {
            leftClickInputUsedThisFrame = true;
            return Input.GetMouseButtonDown(0);
        }
        else
        {
            return false;
        }
    }
}
