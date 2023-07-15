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

        playerActionDisablingUIManager.OnActionDisablingUIOpenSet +=
            PlayerActionDisablingUIManager_OnActionDisablingUIOpenSet;
        targetInventory.OnItemAdded += TargetInventory_OnItemAdded;
        targetInventory.OnItemRemoved += TargetInventory_OnItemRemoved;
    }

    private void OnDestroy()
    {
        playerActionDisablingUIManager.OnActionDisablingUIOpenSet -=
            PlayerActionDisablingUIManager_OnActionDisablingUIOpenSet;

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

        if (itemNameToAdditionText.TryGetValue(item.itemData.name,
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

            itemNameToAdditionText.Add(item.itemData.name, spawnedAdditionTextController);
        }
    }

    private void TargetInventory_OnItemRemoved(ItemWithAmount item)
    {
        if (PauseController.Instance.GamePaused)
        {
            return;
        }

        if (itemNameToAdditionText.TryGetValue(item.itemData.name,
            out InventoryAdditionTextUIController additionTextController))
        {
            additionTextController.RemoveAmount(item.amount);
        }
    }

    private void PlayerActionDisablingUIManager_OnActionDisablingUIOpenSet()
    {
        inventoryAdditionUICanvasGroup.alpha =
            playerActionDisablingUIManager.ActionDisablingUIOpen ? 0f : 1f;
    }

    public void OnAdditionTextDestroyed(string itemName)
    {
        itemNameToAdditionText.Remove(itemName);
    }
}
