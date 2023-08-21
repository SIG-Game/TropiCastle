using System.Collections.Generic;
using UnityEngine;

public class TradingNPC : NPCInteractable
{
    [SerializeField] private NPCTradeScriptableObject trade;
    [SerializeField] private List<string> noInputItemDialogue;
    [SerializeField] private List<string> playerInventoryFullDialogue;

    public override void Interact(PlayerController player)
    {
        FacePlayer(player);

        Inventory playerInventory = player.GetInventory();
        List<ItemWithAmount> itemList = playerInventory.GetItemList();

        int inputItemStackIndex = itemList.FindIndex(
            x => x.itemData == trade.InputItem);

        if (inputItemStackIndex == -1)
        {
            DialogueBox.Instance.PlayDialogue(noInputItemDialogue);

            return;
        }

        if (playerInventory.CanAddItem(trade.OutputItem))
        {
            playerInventory.DecrementItemStackAtIndex(inputItemStackIndex);
            playerInventory.AddItem(trade.OutputItem);
        }
        else
        {
            DialogueBox.Instance.PlayDialogue(playerInventoryFullDialogue);
        }
    }
}
