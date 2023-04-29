public class DefaultItemPickupAndPlacementState : BaseItemPickupAndPlacementState
{
    public DefaultItemPickupAndPlacementState(ItemPickupAndPlacement itemPickupAndPlacement) :
        base(itemPickupAndPlacement)
    {
    }

    public override void StateEnter()
    {
    }

    public override void StateUpdate()
    {
        if (itemPickupAndPlacement.CursorIsOverItemWorld())
        {
            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.PickupState);
        }
        else if (itemPickupAndPlacement.PlacingItem())
        {
            itemPickupAndPlacement.SwitchState(itemPickupAndPlacement.PlacementState);
        }
    }

    public override void StateExit()
    {
    }
}
