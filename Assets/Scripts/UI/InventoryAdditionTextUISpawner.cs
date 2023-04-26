using UnityEngine;

public class InventoryAdditionTextUISpawner : MonoBehaviour
{
    [SerializeField] private GameObject inventoryAdditionText;
    [SerializeField] private Inventory targetInventory;
    [SerializeField] private float verticalSpacing;

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
        MoveChildInventoryAdditionTextUp();

        GameObject spawnedInventoryAdditionText =
            Instantiate(inventoryAdditionText, transform);

        spawnedInventoryAdditionText.GetComponent<InventoryAdditionTextUIController>()
            .SetText($"+{item.amount} {item.itemData.name}");
    }

    private void MoveChildInventoryAdditionTextUp()
    {
        foreach (RectTransform childInventoryAdditionText in transform)
        {
            childInventoryAdditionText.anchoredPosition =
                new Vector2(childInventoryAdditionText.anchoredPosition.x,
                    childInventoryAdditionText.anchoredPosition.y + verticalSpacing);
        }
    }
}
