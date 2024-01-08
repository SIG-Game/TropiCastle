using System.Collections.Generic;
using UnityEngine;

public class BuyerNPC : NPCInteractable
{
    [SerializeField] private ItemScriptableObject itemToBuyDefinition;
    [SerializeField] private int buyCost;
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
            x => x.ItemDefinition == itemToBuyDefinition);

        if (itemToBuyIndex != -1)
        {
            playerInventory.DecrementItemStackAtIndex(itemToBuyIndex);

            playerMoneyController.Money += buyCost;

            dialogueBox.PlayDialogue(
                $"+{buyCost} money for 1 {itemToBuyDefinition.DisplayName}.",
                directionController.UseDefaultDirection);
        }
        else
        {
            dialogueBox.PlayDialogue(
                $"I will buy {itemToBuyDefinition.DisplayName} items from you.",
                directionController.UseDefaultDirection);
        }
    }
}
