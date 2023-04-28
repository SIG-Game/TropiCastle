using UnityEngine.InputSystem;

public class ItemPickupState : BaseItemPickupAndPlacementState
{
    private InputAction itemPickupAndPlacementAction;

    public bool ResetCursorOnPickupInputRelease { get; set; }

    public ItemPickupState(ItemPickupAndPlacement itemPickupAndPlacement,
        InputAction itemPickupAndPlacementAction) :
        base(itemPickupAndPlacement)
    {
        this.itemPickupAndPlacementAction = itemPickupAndPlacementAction;
    }

    public override void StateEnter()
    {
        itemPickupAndPlacement.UsePickupCursor();

        // Pick up hovered item on state entry if needed
        UseInputForPickup();
    }

    public override void StateUpdate()
    {
        if (!itemPickupAndPlacement.MouseIsOverItemWorld())
        {
            if (itemPickupAndPlacement.PlacingItem())
            {
                itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.PlacementState);
            }
            else
            {
                itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);
            }

            return;
        }

        UseInputForPickup();
    }

    private void UseInputForPickup()
    {
        bool holdingPickupInputAndNotPlacingItem = itemPickupAndPlacementAction.IsPressed() &&
            !itemPickupAndPlacement.PlacingItem();

        if (itemPickupAndPlacementAction.WasPressedThisFrame() ||
            holdingPickupInputAndNotPlacingItem)
        {
            itemPickupAndPlacement.PickUpHoveredItem();

            // Prevent item placement on input release from using the
            // same press as item pickup on input press or input hold
            itemPickupAndPlacement.WaitingForInputReleaseBeforePlacement = true;

            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);
        }
        else if (itemPickupAndPlacementAction.WasReleasedThisFrame())
        {
            itemPickupAndPlacement.PickUpHoveredItem();

            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);
        }
    }

    public override void StateExit()
    {
        bool placingItemOrNotHoldingPickup = itemPickupAndPlacement.PlacingItem() ||
            !itemPickupAndPlacementAction.IsPressed();

        if (placingItemOrNotHoldingPickup)
        {
            itemPickupAndPlacement.ResetCursor();
        }
        else
        {
            ResetCursorOnPickupInputRelease = true;
        }
    }
}
