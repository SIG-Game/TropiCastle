using UnityEngine;

public class BucketItemUsage : MonoBehaviour, IItemUsage
{
    private ItemWithAmount bucketOfWaterItem;
    private LayerMask waterMask;

    private void Awake()
    {
        bucketOfWaterItem = new ItemWithAmount
        {
            itemData = Resources.Load<ItemScriptableObject>("Items/BucketOfWater"),
            amount = 1
        };

        waterMask = LayerMask.GetMask("Water");
    }

    public void UseItem(PlayerController playerController)
    {
        if (playerController.InteractionCast(waterMask, 0.25f).collider != null)
        {
            Inventory playerInventory = playerController.GetInventory();

            int selectedItemIndex = playerController.GetSelectedItemIndex();

            playerInventory.RemoveItemAtIndex(selectedItemIndex);
            playerInventory.AddItemAtIndexWithFallbackToFirstEmptyIndex(
                bucketOfWaterItem, selectedItemIndex);
        }
    }
}
