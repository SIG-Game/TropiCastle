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

        previousHoveredItemWorld = itemPickupAndPlacement.GetHoveredItemWorld();
    }

    public override void StateUpdate()
    {
        if (itemPickupAndPlacement.GetHoveredItemWorld() != previousHoveredItemWorld)
        {
            itemPickupAndPlacement.UsePickupCursor();
        }

        if (itemPickupAndPlacement.CursorIsOverItemWorld())
        {
            PickUpHoveredItemUsingInput();
        }
        else if (itemPickupAndPlacement.PlacingItem())
        {
            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.PlacementState);
        }
        else if (!itemPickupAndPlacementAction.IsPressed())
        {
            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);
        }

        previousHoveredItemWorld = itemPickupAndPlacement.GetHoveredItemWorld();
    }

    private void PickUpHoveredItemUsingInput()
    {
        bool holdingPickupInputAndNotPlacingItem = itemPickupAndPlacementAction.IsPressed() &&
            !itemPickupAndPlacement.PlacingItem();

        if (itemPickupAndPlacementAction.WasPressedThisFrame() ||
            holdingPickupInputAndNotPlacingItem)
        {
            itemPickupAndPlacement.PickUpHoveredItem();

            // Prevent held input from being used for item placement
            itemPickupAndPlacement.WaitingForInputReleaseBeforePlacement = true;
        }
        else if (itemPickupAndPlacementAction.WasReleasedThisFrame())
        {
            itemPickupAndPlacement.PickUpHoveredItem();

            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);
        }
    }

    public override void StateExit()
    {
        itemPickupAndPlacement.ResetCursor();
    }
}
