public abstract class BaseItemPickupAndPlacementState
{
    protected ItemPickupAndPlacement itemPickupAndPlacement;

    public BaseItemPickupAndPlacementState(ItemPickupAndPlacement itemPickupAndPlacement)
    {
        this.itemPickupAndPlacement = itemPickupAndPlacement;
    }

    public abstract void StateEnter();

    public abstract void StateUpdate();

    public abstract void StateExit();
}
