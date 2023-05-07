using UnityEngine;

public class BucketItemUsage : IItemUsage
{
    private ItemWithAmount bucketOfWaterItem;
    private LayerMask waterMask;

    public void UseItem(PlayerController playerController)
    {
        if (bucketOfWaterItem == null && waterMask == 0)
        {
            SetUpBucketItemUsage();
        }

        if (playerController.InteractionCast(waterMask, 0.25f).collider != null)
        {
            Inventory playerInventory = playerController.GetInventory();

            int selectedItemIndex = playerController.GetSelectedItemIndex();

            playerInventory.RemoveItemAtIndex(selectedItemIndex);
            playerInventory.AddItemAtIndexWithFallbackToFirstEmptyIndex(
                bucketOfWaterItem, selectedItemIndex);
        }
    }

    private void SetUpBucketItemUsage()
    {
        // Resources.Load cannot be called in the BucketItemUsage constructor
        bucketOfWaterItem = new ItemWithAmount
        {
            itemData = Resources.Load<ItemScriptableObject>("Items/BucketOfWater"),
            amount = 1
        };

        // LayerMask.GetMask cannot be called in the BucketItemUsage constructor
        waterMask = LayerMask.GetMask("Water");
    }
}
