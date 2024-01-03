using UnityEngine;

public class SellerNPC : NPCInteractable
{
    [SerializeField] private ItemStack itemToSell;
    [SerializeField] private int cost;
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

        if (playerMoneyController.Money < cost)
        {
            dialogueBox.PlayDialogue("You have insufficient funds.",
                AfterDialogueAction);
        }
        else if (!playerInventory.CanAddItem(itemToSell))
        {
            dialogueBox.PlayDialogue("Your inventory is full.",
                AfterDialogueAction);
        }
        else
        {
            playerInventory.AddItem(itemToSell);

            playerMoneyController.Money -= cost;

            dialogueBox.PlayDialogue($"-{cost} money for {itemToSell}.",
                AfterDialogueAction);
        }
    }

    private void AfterDialogueAction()
    {
        directionController.UseDefaultDirection();
    }
}
