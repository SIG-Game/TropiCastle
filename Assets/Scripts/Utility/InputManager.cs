using System;
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

    // Returns input bool when that input has not been used this frame
    // Otherwise, returns false
    private static bool GetInputIfUnusedThisFrame(Func<bool> inputFunc, ref bool inputUsedThisFrame)
    {
        if (!inputUsedThisFrame)
        {
            inputUsedThisFrame = true;
            return inputFunc();
        }
        else
        {
            return false;
        }
    }

    public bool GetLeftClickDownIfUnusedThisFrame() =>
        GetInputIfUnusedThisFrame(() => Input.GetMouseButtonDown(0), ref leftClickDownUsedThisFrame);

    public bool GetInteractButtonDownIfUnusedThisFrame() =>
        GetInputIfUnusedThisFrame(() => Input.GetButtonDown("Interact"), ref interactButtonDownUsedThisFrame);
}
