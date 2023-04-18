using UnityEngine;

public class ItemPickupState : BaseItemPickupAndPlacementState
{
    public ItemPickupState(ItemPickupAndPlacement itemPickupAndPlacement) :
        base(itemPickupAndPlacement)
    {
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
        if (Input.GetMouseButtonDown(1))
        {
            itemPickupAndPlacement.PickUpHoveredItem();

            // Prevent item placement on right-click release from using
            // the same click as item pickup on right-click press
            itemPickupAndPlacement.WaitingForRightClickReleaseBeforePlacement = true;

            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.DefaultState);
        }
        else if (Input.GetMouseButtonUp(1))
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
