using System.Collections.Generic;
using UnityEngine;

public class BuyerNPC : NPCInteractable
{
    [SerializeField] private NPCPurchaseScriptableObject purchase;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private MoneyController playerMoneyController;

    [Inject] private DialogueBox dialogueBox;
    [Inject] private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();
    }

    public override void Interact()
    {
        FacePlayer(playerController);

        List<ItemStack> playerItemList = playerInventory.GetItemList();

        int itemToBuyIndex = playerItemList.FindIndex(
            x => x.ItemDefinition == purchase.ItemDefinition);

        if (itemToBuyIndex != -1)
        {
            playerInventory.DecrementItemStackAtIndex(itemToBuyIndex);

            playerMoneyController.Money += purchase.Payment;

            dialogueBox.PlayDialogue($"+{purchase.Payment} money " +
                    $"for 1 {purchase.ItemDefinition.DisplayName}.",
                directionController.UseDefaultDirection);
        }
        else
        {
            dialogueBox.PlayDialogue($"I will buy " +
                    $"{purchase.ItemDefinition.DisplayName} items from you.",
                directionController.UseDefaultDirection);
        }
    }
}
