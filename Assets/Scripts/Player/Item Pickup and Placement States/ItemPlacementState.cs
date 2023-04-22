using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPlacementState : BaseItemPickupAndPlacementState
{
    private InputAction itemPickupAndPlacementAction;

    public ItemPlacementState(ItemPickupAndPlacement itemPickupAndPlacement,
        InputAction itemPickupAndPlacementAction) :
        base(itemPickupAndPlacement)
    {
        this.itemPickupAndPlacementAction = itemPickupAndPlacementAction;
    }

    public override void StateEnter()
    {
        // Use placement graphics on state entry if needed
        StateUpdate();
    }

    public override void StateUpdate()
    {
        if (itemPickupAndPlacement.MouseIsOverItemWorld())
        {
            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.PickupState);

            return;
        }

        if (!MouseIsOnScreen())
        {
            itemPickupAndPlacement.ResetCursorAndPlayerItem();

            if (itemPickupAndPlacementAction.WasReleasedThisFrame())
            {
                itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);
            }

            return;
        }

        itemPickupAndPlacement.UsePlacementCursorAndPlayerItem();

        if (itemPickupAndPlacementAction.WasReleasedThisFrame())
        {
            if (itemPickupAndPlacement.CanPlaceItemAtMousePosition())
            {
                itemPickupAndPlacement.PlaceSelectedPlayerHotbarItemAtMousePosition();
            }

            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);

            return;
        }

        // Cancel item placement when left-click is pressed while placing an item
        if (itemPickupAndPlacement.PlacingItem() &&
            InputManager.Instance.GetLeftClickDownIfUnusedThisFrame())
        {
            itemPickupAndPlacement.WaitingForInputReleaseBeforePlacement = true;

            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);

            return;
        }
    }

    public override void StateExit()
    {
        itemPickupAndPlacement.ResetCursorAndPlayerItem();
    }

    private bool MouseIsOnScreen()
    {
        float mouseXPosition = Input.mousePosition.x;
        float mouseYPosition = Input.mousePosition.y;

        bool mouseIsOnScreen = mouseXPosition >= 0f && mouseXPosition <= Screen.width &&
            mouseYPosition >= 0f && mouseYPosition <= Screen.height;

        return mouseIsOnScreen;
    }
}
