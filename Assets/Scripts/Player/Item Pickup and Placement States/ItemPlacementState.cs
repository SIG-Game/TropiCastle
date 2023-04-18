using UnityEngine;

public class ItemPlacementState : BaseItemPickupAndPlacementState
{
    public ItemPlacementState(ItemPickupAndPlacement itemPickupAndPlacement) :
        base(itemPickupAndPlacement)
    {
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

            if (Input.GetMouseButtonUp(1))
            {
                itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);
            }

            return;
        }

        itemPickupAndPlacement.UsePlacementCursorAndPlayerItem();

        if (Input.GetMouseButtonUp(1))
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
            itemPickupAndPlacement.WaitingForRightClickReleaseBeforePlacement = true;

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
