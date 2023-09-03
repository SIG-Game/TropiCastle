using UnityEngine;

public class ThrowableItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Collider2D playerCollider2D;
    [SerializeField] private CursorController cursorController;
    [SerializeField] private float canThrowBoxCastDistance;

    private LayerMask notPlayerLayerMask;

    private void Awake()
    {
        notPlayerLayerMask = ~LayerMask.GetMask("Player");
    }

    public void UseItem(ItemWithAmount item, int itemIndex)
    {
        Vector3 thrownItemStartPosition =
            playerController.transform.position + new Vector3(0f, 0.3f, 0f);

        Vector3 throwDirection =
            ((Vector3)cursorController.GetWorldPosition() - thrownItemStartPosition).normalized;

        Vector2 itemColliderSize =
            ItemWorldPrefabInstanceFactory.GetItemColliderSize(item.itemDefinition);

        Physics2D.queriesHitTriggers = false;
        bool canThrowItem = Physics2D.BoxCast(thrownItemStartPosition, itemColliderSize, 0f,
            throwDirection, canThrowBoxCastDistance, notPlayerLayerMask).collider == null;
        Physics2D.queriesHitTriggers = true;

        if (!canThrowItem)
        {
            return;
        }

        ItemWithAmount itemToThrow = new ItemWithAmount(item);
        itemToThrow.amount = 1;

        ItemWorld thrownItemWorld =
            ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(
                thrownItemStartPosition, itemToThrow);

        ThrownItemWorld thrownItemWorldComponent =
            thrownItemWorld.gameObject.AddComponent<ThrownItemWorld>();

        thrownItemWorldComponent.SetUpThrownItemWorld(throwDirection,
            (ThrowableItemScriptableObject)itemToThrow.itemDefinition, playerCollider2D);

        playerInventory.DecrementItemStackAtIndex(itemIndex);
    }
}
