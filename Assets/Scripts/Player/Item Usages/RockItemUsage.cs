using UnityEngine;

public class RockItemUsage : ThrowableItemUsage
{
    private ItemWithAmount rockItem;

    private void Awake()
    {
        rockItem = new ItemWithAmount(
            Resources.Load<ItemScriptableObject>("Items/Rock"), 1);
    }

    protected override ItemWithAmount GetItemToThrow() => rockItem;
}
