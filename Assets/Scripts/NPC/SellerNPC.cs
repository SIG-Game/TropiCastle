using UnityEngine;

public class SellerNPC : NPCInteractable
{
    [SerializeField] private NPCProductScriptableObject product;
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

        if (playerMoneyController.Money < product.Cost)
        {
            dialogueBox.PlayDialogue("You have insufficient funds.",
                AfterDialogueAction);
        }
        else if (!playerInventory.CanAddItem(product.Item))
        {
            dialogueBox.PlayDialogue("Your inventory is full.",
                AfterDialogueAction);
        }
        else
        {
            playerInventory.AddItem(product.Item);

            playerMoneyController.Money -= product.Cost;

            dialogueBox.PlayDialogue($"-{product.Cost} money for {product.Item}.",
                AfterDialogueAction);
        }
    }

    private void AfterDialogueAction()
    {
        directionController.UseDefaultDirection();
    }
}
