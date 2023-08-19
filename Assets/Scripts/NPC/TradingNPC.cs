using System.Collections.Generic;
using UnityEngine;

public class TradingNPC : NPCInteractable
{
    [SerializeField] private ItemScriptableObject inputItem;
    [SerializeField] private ItemWithAmount outputItem;
    [SerializeField] private List<string> noInputItemDialogue;
    [SerializeField] private List<string> playerInventoryFullDialogue;

    public override void Interact(PlayerController player)
    {
        FacePlayer(player);

        Inventory playerInventory = player.GetInventory();
        List<ItemWithAmount> itemList = playerInventory.GetItemList();

        int inputItemStackIndex = itemList.FindIndex(x => x.itemData == inputItem);

        if (inputItemStackIndex == -1)
        {
            DialogueBox.Instance.PlayDialogue(noInputItemDialogue);

            return;
        }

        if (playerInventory.CanAddItem(outputItem))
        {
            playerInventory.DecrementItemStackAtIndex(inputItemStackIndex);
            playerInventory.AddItem(outputItem);
        }
        else
        {
            DialogueBox.Instance.PlayDialogue(playerInventoryFullDialogue);
        }
    }
}
