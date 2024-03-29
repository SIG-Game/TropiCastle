using UnityEngine.InputSystem;

public class ItemPlacementState : BaseItemPickupAndPlacementState
{
    private InputManager inputManager;
    private InputAction itemPickupAndPlacementAction;

    public ItemPlacementState(ItemPickupAndPlacement itemPickupAndPlacement,
        InputManager inputManager, InputAction itemPickupAndPlacementAction) :
        base(itemPickupAndPlacement)
    {
        this.inputManager = inputManager;
        this.itemPickupAndPlacementAction = itemPickupAndPlacementAction;
    }

    public override void StateEnter()
    {
        // Use placement graphics on state entry if needed
        StateUpdate();
    }

    public override void StateUpdate()
    {
        if (itemPickupAndPlacement.CursorIsOverItemWorld)
        {
            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.PickupState);
            return;
        }

        if (itemPickupAndPlacement.SelectedItemIsEmpty())
        {
            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);
            return;
        }

        itemPickupAndPlacement.UsePlacementCursorAndPlayerItem();

        if (itemPickupAndPlacementAction.WasReleasedThisFrame())
        {
            if (itemPickupAndPlacement.CanPlaceItemAtCursorPosition)
            {
                itemPickupAndPlacement.PlaceSelectedItemAtCursorPosition();
            }

            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);
        }
        // Cancel item placement when use item button is pressed while placing an item
        else if (itemPickupAndPlacement.PlacingItem &&
            inputManager.GetUseItemButtonDownIfUnusedThisFrame())
        {
            itemPickupAndPlacement.WaitingForInputReleaseBeforePlacement = true;

            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);
        }
    }

    public override void StateExit()
    {
        itemPickupAndPlacement.ResetCursorAndPlayerItem();
    }
}
