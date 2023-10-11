using System.Collections.Generic;
using UnityEngine;

public class InventoryAdditionTextUISpawner : MonoBehaviour
{
    [SerializeField] private GameObject inventoryAdditionText;
    [SerializeField] private CanvasGroup inventoryAdditionUICanvasGroup;
    [SerializeField] private Inventory targetInventory;
    [SerializeField] private PlayerActionDisablingUIManager playerActionDisablingUIManager;

    private Dictionary<string, InventoryAdditionTextUIController> itemNameToAdditionText;

    private void Awake()
    {
        itemNameToAdditionText = new Dictionary<string, InventoryAdditionTextUIController>();

        playerActionDisablingUIManager.OnUIOpened +=
            PlayerActionDisablingUIManager_OnUIOpened;
        playerActionDisablingUIManager.OnUIClosed +=
            PlayerActionDisablingUIManager_OnUIClosed;
        targetInventory.OnItemAdded += TargetInventory_OnItemAdded;
        targetInventory.OnItemRemoved += TargetInventory_OnItemRemoved;
    }

    private void OnDestroy()
    {
        playerActionDisablingUIManager.OnUIOpened -=
            PlayerActionDisablingUIManager_OnUIOpened;
        playerActionDisablingUIManager.OnUIClosed -=
            PlayerActionDisablingUIManager_OnUIClosed;

        if (targetInventory != null)
        {
            targetInventory.OnItemAdded -= TargetInventory_OnItemAdded;
            targetInventory.OnItemRemoved -= TargetInventory_OnItemRemoved;
        }
    }

    private void TargetInventory_OnItemAdded(ItemWithAmount item)
    {
        if (PauseController.Instance.GamePaused)
        {
            return;
        }

        if (itemNameToAdditionText.TryGetValue(item.itemDefinition.name,
            out InventoryAdditionTextUIController additionTextController))
        {
            additionTextController.AddAmount(item.amount);

            additionTextController.transform.SetAsFirstSibling();
        }
        else
        {
            GameObject spawnedAdditionText = Instantiate(inventoryAdditionText, transform);

            InventoryAdditionTextUIController spawnedAdditionTextController =
                spawnedAdditionText.GetComponent<InventoryAdditionTextUIController>();

            spawnedAdditionTextController.UpdateWithItem(item);
            spawnedAdditionTextController.SetSpawner(this);

            // Ensure spawned inventory addition text is at the
            // bottom of the inventory addition UI
            spawnedAdditionText.transform.SetAsFirstSibling();

            itemNameToAdditionText.Add(item.itemDefinition.name, spawnedAdditionTextController);
        }
    }

    private void TargetInventory_OnItemRemoved(ItemWithAmount item)
    {
        if (PauseController.Instance.GamePaused)
        {
            return;
        }

        if (itemNameToAdditionText.TryGetValue(item.itemDefinition.name,
            out InventoryAdditionTextUIController additionTextController))
        {
            additionTextController.RemoveAmount(item.amount);
        }
    }

    private void PlayerActionDisablingUIManager_OnUIOpened()
    {
        inventoryAdditionUICanvasGroup.alpha = 0f;
    }

    private void PlayerActionDisablingUIManager_OnUIClosed()
    {
        inventoryAdditionUICanvasGroup.alpha = 1f;
    }

    public void OnAdditionTextDestroyed(string itemName)
    {
        itemNameToAdditionText.Remove(itemName);
    }
}
