using UnityEngine;

public class HealingItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private HealthController playerHealthController;
    [SerializeField] private Inventory playerInventory;

    public void UseItem(ItemWithAmount item, int itemIndex)
    {
        if (!playerHealthController.AtMaxHealth())
        {
            int amountToHeal = ((HealingItemScriptableObject)item.itemData).healAmount;

            playerHealthController.IncreaseHealth(amountToHeal);

            playerInventory.DecrementItemStackAtIndex(itemIndex);
        }
    }

    public void ConsumeFirstHealingItemInPlayerInventory()
    {
        int healingItemIndex = playerInventory.GetItemList().FindIndex(
            x => x.itemData is HealingItemScriptableObject);

        if (healingItemIndex != -1)
        {
            ItemWithAmount healingItem = playerInventory.GetItemAtIndex(healingItemIndex);

            UseItem(healingItem, healingItemIndex);
        }
    }
}
