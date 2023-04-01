﻿using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public bool EscapeKeyUsedThisFrame { get; set; }
    public bool NumberKeyUsedThisFrame { get; set; }

    private PlayerInput playerInput;

    private InputAction interactAction;
    private InputAction fishAction;
    private bool leftClickDownUsedThisFrame;
    private bool interactButtonDownUsedThisFrame;
    private bool fishButtonDownUsedThisFrame;

    private void Awake()
    {
        Instance = this;

        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        interactAction = GetAction("Interact");
        fishAction = GetAction("Fish");
    }

    // Has to run before any scripts that use IfUnusedThisFrame methods
    private void Update()
    {
        EscapeKeyUsedThisFrame = false;
        NumberKeyUsedThisFrame = false;

        leftClickDownUsedThisFrame = false;
        interactButtonDownUsedThisFrame = false;
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

    public bool GetLeftClickDownIfUnusedThisFrame() =>
        GetInputIfUnusedThisFrame(() => Input.GetMouseButtonDown(0), ref leftClickDownUsedThisFrame);

    public bool GetInteractButtonDownIfUnusedThisFrame() =>
        GetInputIfUnusedThisFrame(() => interactAction.WasPressedThisFrame(), ref interactButtonDownUsedThisFrame);

    public bool GetFishButtonDownIfUnusedThisFrame() =>
        GetInputIfUnusedThisFrame(() => fishAction.WasPressedThisFrame(), ref fishButtonDownUsedThisFrame);

    public InputAction GetAction(string actionName) => playerInput.currentActionMap[actionName];
}
