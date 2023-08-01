using UnityEngine.InputSystem;

public class ItemPickupState : BaseItemPickupAndPlacementState
{
    private InputAction itemPickupAndPlacementAction;
    private ItemWorld previousHoveredItemWorld;

    public ItemPickupState(ItemPickupAndPlacement itemPickupAndPlacement,
        InputAction itemPickupAndPlacementAction) :
        base(itemPickupAndPlacement)
    {
        this.itemPickupAndPlacementAction = itemPickupAndPlacementAction;
    }

    public override void StateEnter()
    {
        itemPickupAndPlacement.UsePickupCursor();

        PickUpHoveredItemUsingInput();

        previousHoveredItemWorld = itemPickupAndPlacement.HoveredItemWorld;
    }

    public override void StateUpdate()
    {
        if (itemPickupAndPlacement.HoveredItemWorld != previousHoveredItemWorld)
        {
            itemPickupAndPlacement.UsePickupCursor();
        }

        if (itemPickupAndPlacement.CursorIsOverItemWorld)
        {
            PickUpHoveredItemUsingInput();
        }
        else if (itemPickupAndPlacement.PlacingItem)
        {
            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.PlacementState);
        }
        else if (!itemPickupAndPlacementAction.IsPressed())
        {
            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);
        }

        previousHoveredItemWorld = itemPickupAndPlacement.HoveredItemWorld;
    }

    private void PickUpHoveredItemUsingInput()
    {
        bool holdingPickupInputAndNotPlacingItem = itemPickupAndPlacementAction.IsPressed() &&
            !itemPickupAndPlacement.PlacingItem;

        if (itemPickupAndPlacementAction.WasPressedThisFrame() ||
            holdingPickupInputAndNotPlacingItem)
        {
            itemPickupAndPlacement.AttemptToPickUpHoveredItem();

            // Prevent held input from being used for item placement
            itemPickupAndPlacement.WaitingForInputReleaseBeforePlacement = true;
        }
        else if (itemPickupAndPlacementAction.WasReleasedThisFrame())
        {
            itemPickupAndPlacement.AttemptToPickUpHoveredItem();

            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);
        }
    }

    public override void StateExit()
    {
        itemPickupAndPlacement.ResetCursor();
    }
}
