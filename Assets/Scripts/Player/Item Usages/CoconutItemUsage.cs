using UnityEngine;

public class CoconutItemUsage : IItemUsage
{
    private ItemWithAmount coconutItem;
    private CursorController cursorController;

    public void SetCursorController(CursorController cursorController)
    {
        this.cursorController = cursorController;
    }

    public void UseItem(PlayerController playerController)
    {
        coconutItem = new ItemWithAmount
        {
            itemData = Resources.Load<ItemScriptableObject>("Items/Coconut"),
            amount = 1
        };

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

        playerController.GetInventory()
            .RemoveItemAtIndex(playerController.GetSelectedItemIndex());
    }
}
