using UnityEngine;

public class BucketItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;

    private ItemWithAmount bucketOfWaterItem;
    private LayerMask waterMask;

    private void Awake()
    {
        bucketOfWaterItem = new ItemWithAmount(
            Resources.Load<ItemScriptableObject>("Items/BucketOfWater"), 1);

        waterMask = LayerMask.GetMask("Water");
    }

    public void UseItem(ItemWithAmount _, int itemIndex)
    {
        if (playerController.InteractionCast(waterMask, 0.25f, 0.2f).collider != null)
        {
            playerInventory.RemoveItemAtIndex(itemIndex);
            playerInventory.AddItemAtIndexWithFallbackToFirstEmptyIndex(
                bucketOfWaterItem, itemIndex);
        }
    }
}
