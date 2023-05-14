using UnityEngine;

public class CoconutItemUsage : MonoBehaviour, IItemUsage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private CursorController cursorController;

    private ItemWithAmount coconutItem;

    private void Awake()
    {
        coconutItem = new ItemWithAmount
        {
            itemData = Resources.Load<ItemScriptableObject>("Items/Coconut"),
            amount = 1
        };
    }

    public void UseItem()
    {
        Vector3 coconutStartPosition =
            playerController.transform.position + new Vector3(0f, 0.3f, 0f);

        ItemWorld coconutItemWorld =
            ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(
                coconutStartPosition, coconutItem);

        ThrownCoconutItemWorld thrownCoconutItemWorld =
            coconutItemWorld.gameObject.AddComponent<ThrownCoconutItemWorld>();

        Vector3 coconutThrowDirection =
            ((Vector3)cursorController.GetPosition() - coconutStartPosition).normalized;

        thrownCoconutItemWorld.SetUpThrownCoconutItemWorld(coconutThrowDirection);

        playerInventory.RemoveItemAtIndex(playerController.GetSelectedItemIndex());
    }
}
