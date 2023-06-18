using UnityEngine;

public class ThrowableItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private CursorController cursorController;

    public void UseItem(ItemWithAmount item, int itemIndex)
    {
        ItemWithAmount itemToThrow = new ItemWithAmount(item);
        itemToThrow.amount = 1;

        Vector3 thrownItemStartPosition =
            playerController.transform.position + new Vector3(0f, 0.3f, 0f);

        ItemWorld thrownItemWorld =
            ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(
                thrownItemStartPosition, itemToThrow);

        ThrownItemWorld thrownItemWorldComponent =
            thrownItemWorld.gameObject.AddComponent<ThrownItemWorld>();

        Vector3 throwDirection =
            ((Vector3)cursorController.GetWorldPosition() - thrownItemStartPosition).normalized;

        thrownItemWorldComponent.SetUpThrownItemWorld(throwDirection,
            (ThrowableItemScriptableObject)itemToThrow.itemData);

        playerInventory.DecrementItemStackAtIndex(itemIndex);
    }
}
