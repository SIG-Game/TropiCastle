using System.Collections.Generic;
using UnityEngine;

public class InventoryAdditionTextUISpawner : MonoBehaviour
{
    [SerializeField] private GameObject inventoryAdditionText;
    [SerializeField] private Inventory targetInventory;

    private Dictionary<string, InventoryAdditionTextUIController> itemNameToAdditionText;

    private void Awake()
    {
        itemNameToAdditionText = new Dictionary<string, InventoryAdditionTextUIController>();

        targetInventory.OnItemAdded += TargetInventory_OnItemAdded;
    }

    private void OnDestroy()
    {
        if (targetInventory != null)
        {
            targetInventory.OnItemAdded -= TargetInventory_OnItemAdded;
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

    public void OnAdditionTextDestroyed(string itemName)
    {
        itemNameToAdditionText.Remove(itemName);
    }
}
