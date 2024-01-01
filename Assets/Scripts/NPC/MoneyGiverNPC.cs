using UnityEngine;

public class MoneyGiverNPC : NPCInteractable
{
    [SerializeField] private MoneyController playerMoneyController;

    [Inject] private DialogueBox dialogueBox;

    private const int giveAmount = 10;

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();
    }

    public override void Interact(PlayerController player)
    {
        playerMoneyController.Money += giveAmount;

        FacePlayer(player);

        dialogueBox.PlayDialogue($"+{giveAmount} money.", AfterDialogueAction);
    }

    private void AfterDialogueAction()
    {
        directionController.UseDefaultDirection();
    }
}
