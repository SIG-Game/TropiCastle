using UnityEngine;

public class ThrowableItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private CursorController cursorController;

    public void UseItem()
    {
        ItemWithAmount itemToThrow = new ItemWithAmount(playerController.GetSelectedItem());
        itemToThrow.amount = 1;

        int damage = ((ThrowableItemScriptableObject)itemToThrow.itemData).damage;

        Vector3 thrownItemStartPosition =
            playerController.transform.position + new Vector3(0f, 0.3f, 0f);

        ItemWorld thrownItemWorld =
            ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(
                thrownItemStartPosition, itemToThrow);

        ThrownItemWorld thrownItemWorldComponent =
            thrownItemWorld.gameObject.AddComponent<ThrownItemWorld>();

        thrownItemWorldComponent.SetDamage(damage);

        Vector3 throwDirection =
            ((Vector3)cursorController.GetWorldPosition() - thrownItemStartPosition).normalized;

        thrownItemWorldComponent.SetUpThrownItemWorld(throwDirection);

        playerInventory.DecrementItemStackAtIndex(playerController.GetSelectedItemIndex());
    }
}
