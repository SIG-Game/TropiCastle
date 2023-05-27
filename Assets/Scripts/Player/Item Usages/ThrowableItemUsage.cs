using UnityEngine;

public abstract class ThrowableItemUsage : MonoBehaviour,IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private CursorController cursorController;
    [SerializeField] private int damage;

    public void UseItem()
    {
        Vector3 thrownItemStartPosition =
            playerController.transform.position + new Vector3(0f, 0.3f, 0f);

        ItemWorld thrownItemWorld =
            ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(
                thrownItemStartPosition, GetItemToThrow());

        ThrownItemWorld thrownItemWorldComponent =
            thrownItemWorld.gameObject.AddComponent<ThrownItemWorld>();

        thrownItemWorldComponent.SetDamage(damage);

        Vector3 throwDirection =
            ((Vector3)cursorController.GetWorldPosition() - thrownItemStartPosition).normalized;

        thrownItemWorldComponent.SetUpThrownItemWorld(throwDirection);

        playerInventory.DecrementItemStackAtIndex(playerController.GetSelectedItemIndex());
    }

    protected abstract ItemWithAmount GetItemToThrow();
}
