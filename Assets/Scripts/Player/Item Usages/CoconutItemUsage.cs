using UnityEngine;

public class CoconutItemUsage : ThrowableItemUsage
{
    private ItemWithAmount coconutItem;

    private void Awake()
    {
        coconutItem = new ItemWithAmount(
            Resources.Load<ItemScriptableObject>("Items/Coconut"), 1);
    }

    protected override ItemWithAmount GetItemToThrow() => coconutItem;
}
