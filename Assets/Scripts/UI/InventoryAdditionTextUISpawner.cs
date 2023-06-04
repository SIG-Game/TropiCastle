using UnityEngine;

public class InventoryAdditionTextUISpawner : MonoBehaviour
{
    [SerializeField] private GameObject inventoryAdditionText;
    [SerializeField] private Inventory targetInventory;

    private void Awake()
    {
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

        GameObject spawnedInventoryAdditionText =
            Instantiate(inventoryAdditionText, transform);

        spawnedInventoryAdditionText.GetComponent<InventoryAdditionTextUIController>()
            .SetText($"+{item.amount} {item.itemData.name}");

        // Ensure spawned inventory addition text is at the bottom of the inventory addition UI
        spawnedInventoryAdditionText.transform.SetAsFirstSibling();
    }
}
