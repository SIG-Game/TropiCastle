using System.Collections.Generic;
using UnityEngine;

public class BuyerNPC : NPCInteractable
{
    [SerializeField] private ItemScriptableObject itemToBuyDefinition;
    [SerializeField] private int buyCost;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private MoneyController playerMoneyController;

    [Inject] private DialogueBox dialogueBox;

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();
    }

    public override void Interact(PlayerController player)
    {
        FacePlayer(player);

        List<ItemStack> playerItemList = playerInventory.GetItemList();

        int itemToBuyIndex = playerItemList.FindIndex(
            x => x.ItemDefinition == itemToBuyDefinition);

        if (itemToBuyIndex != -1)
        {
            playerInventory.DecrementItemStackAtIndex(itemToBuyIndex);

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
