using UnityEngine;

public class RockItemUsage : ThrowableItemUsage
{
    private ItemWithAmount rockItem;

    private void Awake()
    {
        rockItem = new ItemWithAmount
        {
            itemData = Resources.Load<ItemScriptableObject>("Items/Rock"),
            amount = 1
        };
    }

    protected override ItemWithAmount GetItemToThrow() => rockItem;
}
