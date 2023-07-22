using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionReference useItemActionReference;
    [SerializeField] private InputActionReference interactActionReference;
    [SerializeField] private InputActionReference inventoryActionReference;
    [SerializeField] private InputActionReference fishActionReference;

    public static InputManager Instance;

    private PlayerInput playerInput;

    private InputAction useItemAction;
    private InputAction interactAction;
    private InputAction inventoryAction;
    private InputAction fishAction;
    private bool numberKeyUsedThisFrame;
    private bool useItemButtonDownUsedThisFrame;
    private bool interactButtonDownUsedThisFrame;
    private bool inventoryButtonDownUsedThisFrame;
    private bool fishButtonDownUsedThisFrame;

    private void Awake()
    {
        Instance = this;

        playerInput = GetComponent<PlayerInput>();

        useItemAction = useItemActionReference.action;
        interactAction = interactActionReference.action;
        inventoryAction = inventoryActionReference.action;
        fishAction = fishActionReference.action;
    }

    // Has to run before any scripts that use IfUnusedThisFrame methods
    private void Update()
    {
        numberKeyUsedThisFrame = false;
        useItemButtonDownUsedThisFrame = false;
        interactButtonDownUsedThisFrame = false;
        inventoryButtonDownUsedThisFrame = false;
        fishButtonDownUsedThisFrame = false;
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

    public int? GetNumberKeyIndexIfUnusedThisFrame()
    {
        if (numberKeyUsedThisFrame)
        {
            return null;
        }

        numberKeyUsedThisFrame = true;

        int? numberKeyIndex;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            numberKeyIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            numberKeyIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            numberKeyIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            numberKeyIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            numberKeyIndex = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            numberKeyIndex = 5;
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            numberKeyIndex = 6;
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            numberKeyIndex = 7;
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            numberKeyIndex = 8;
        else if (Input.GetKeyDown(KeyCode.Alpha0))
            numberKeyIndex = 9;
        else
            numberKeyIndex = null;

        return numberKeyIndex;
    }

    public void DisableGettingNumberKeyInputThisFrame()
    {
        numberKeyUsedThisFrame = true;
    }

    public bool GetUseItemButtonDownIfUnusedThisFrame() =>
        GetInputIfUnusedThisFrame(() => useItemAction.WasPressedThisFrame(),
            ref useItemButtonDownUsedThisFrame);

    public bool GetInteractButtonDownIfUnusedThisFrame() =>
        GetInputIfUnusedThisFrame(() => interactAction.WasPressedThisFrame(),
            ref interactButtonDownUsedThisFrame);

    public bool GetInventoryButtonDownIfUnusedThisFrame() =>
        GetInputIfUnusedThisFrame(() => inventoryAction.WasPressedThisFrame(),
            ref inventoryButtonDownUsedThisFrame);

    public bool GetFishButtonDownIfUnusedThisFrame() =>
        GetInputIfUnusedThisFrame(() => fishAction.WasPressedThisFrame(),
            ref fishButtonDownUsedThisFrame);

    public string GetCurrentControlScheme() => playerInput.currentControlScheme;

    public PlayerInput GetPlayerInput() => playerInput;
}
