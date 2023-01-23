using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private bool leftClickDownUsedThisFrame;
    private bool interactButtonDownUsedThisFrame;

    private void Awake()
    {
        Instance = this;
    }

    // Has to run before any scripts that use IfUnusedThisFrame methods
    private void Update()
    {
        leftClickDownUsedThisFrame = false;
        interactButtonDownUsedThisFrame = false;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    // Returns left-click down bool when that input has not been used this frame
    // Otherwise, returns false
    public bool GetLeftClickDownIfUnusedThisFrame()
    {
        if (!leftClickDownUsedThisFrame)
        {
            leftClickDownUsedThisFrame = true;
            return Input.GetMouseButtonDown(0);
        }
        else
        {
            return false;
        }
    }

    public bool GetInteractButtonDownIfUnusedThisFrame()
    {
        if (!interactButtonDownUsedThisFrame)
        {
            interactButtonDownUsedThisFrame = true;
            return Input.GetButtonDown("Interact");
        }
        else
        {
            return false;
        }
    }
}
