using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoHealButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button autoHealButton;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private HealthController playerHealthController;

    private Tooltip maxHealthTooltip;

    private void Awake()
    {
        maxHealthTooltip = new Tooltip("Already at max health.", 0);

        playerHealthController.OnHealthChanged += HealthController_OnHealthChanged;
    }

    private void OnDestroy()
    {
        if (playerHealthController != null)
        {
            playerHealthController.OnHealthChanged -= HealthController_OnHealthChanged;
        }
    }

    public void AutoHealButton_OnClick()
    {
        if (playerHealthController.AtMaxHealth())
        {
            return;
        }

        List<ItemWithAmount> playerInventoryItemList = playerInventory.GetItemList();

        for (int i = 0; i < playerInventoryItemList.Count; ++i)
        {
            if (playerInventoryItemList[i].itemData is HealingItemScriptableObject healingItem)
            {
                playerInventory.RemoveItemAtIndex(i);
                playerHealthController.IncreaseHealth(healingItem.healAmount);

                if (playerHealthController.AtMaxHealth())
                {
                    return;
                }
            }
        }
    }

    private void HealthController_OnHealthChanged(int _, int _1)
    {
        autoHealButton.interactable = !playerHealthController.AtMaxHealth();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playerHealthController.AtMaxHealth())
        {
            InventoryUITooltipController.Instance.AddTooltipTextWithPriority(maxHealthTooltip);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (playerHealthController.AtMaxHealth())
        {
            InventoryUITooltipController.Instance.RemoveTooltipTextWithPriority(maxHealthTooltip);
        }
    }
}
