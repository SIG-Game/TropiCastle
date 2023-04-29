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
        if (itemPickupAndPlacement.CursorIsOverItemWorld())
        {
            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.PickupState);

            return;
        }

        itemPickupAndPlacement.UsePlacementCursorAndPlayerItem();

        if (itemPickupAndPlacementAction.WasReleasedThisFrame())
        {
            if (itemPickupAndPlacement.CanPlaceItemAtCursorPosition())
            {
                itemPickupAndPlacement.PlaceSelectedPlayerHotbarItemAtCursorPosition();
            }

            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);

            return;
        }

        // Cancel item placement when use item button is pressed while placing an item
        if (itemPickupAndPlacement.PlacingItem() &&
            InputManager.Instance.GetUseItemButtonDownIfUnusedThisFrame())
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
}
