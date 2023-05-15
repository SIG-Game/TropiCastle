using UnityEngine;

public class CoconutItemUsage : ThrowableItemUsage
{
    private ItemWithAmount coconutItem;

    private void Awake()
    {
        coconutItem = new ItemWithAmount
        {
            itemData = Resources.Load<ItemScriptableObject>("Items/Coconut"),
            amount = 1
        };
    }

    protected override ItemWithAmount GetItemToThrow() => coconutItem;
}
