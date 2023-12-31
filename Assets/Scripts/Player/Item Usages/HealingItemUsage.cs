using UnityEngine;

public class HealingItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private HealthController playerHealthController;
    [SerializeField] private Inventory playerInventory;

    public void UseItem(ItemStack item, int itemIndex)
    {
        if (!playerHealthController.AtMaxHealth)
        {
            int amountToHeal = item.ItemDefinition.GetIntProperty("HealAmount");

            playerHealthController.Health += amountToHeal;

            playerInventory.DecrementItemStackAtIndex(itemIndex);
        }
    }
}
