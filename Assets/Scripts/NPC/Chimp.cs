using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chimp : NPCInteractable
{
    [SerializeField] private NPCItemOfferingScriptableObject itemOffering;
    [SerializeField] private List<string> givingItemDialogueLines;
    [SerializeField] private List<string> notGivingItemDialogueLines;
    [SerializeField] private List<string> playerInventoryFullDialogueLines;
    [SerializeField] private Vector2 timeBetweenGivesRange;
    [SerializeField] private NPCSpinner chimpSpinner;
    [SerializeField] private CharacterItemInWorldController chimpItemInWorld;

    [Inject] private DialogueBox dialogueBox;

    public float LastGiveTime { get; private set; }
    public float TimeBetweenGives { get; set; }

    private NPCItemOfferingSelector itemOfferingSelector;

    private const string chimpCharacterName = "Chimp";

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();

        itemOfferingSelector = new NPCItemOfferingSelector(itemOffering);

        LastGiveTime = Time.time;
        TimeBetweenGives = 0f;
    }

    private void Start()
    {
        chimpSpinner.StartSpinning();
    }

    public override void Interact(PlayerController player)
    {
        chimpSpinner.StopSpinning();

        FacePlayer(player);

        ItemStack? itemToGive = null;
        List<string> dialogueLinesToPlay = null;

        if (ItemGiveAvailable())
        {
            itemToGive = itemOfferingSelector.SelectItemToGive();

            if (!player.GetInventory().CanAddItem(itemToGive.Value))
            {
                dialogueLinesToPlay = playerInventoryFullDialogueLines;
                itemToGive = null;
            }
            else
            {
                dialogueLinesToPlay = givingItemDialogueLines;
                chimpItemInWorld.ShowItem(itemToGive.Value);
            }
        }
        else
        {
            dialogueLinesToPlay = notGivingItemDialogueLines;
        }

        dialogueBox.SetCharacterName(chimpCharacterName);
        dialogueBox.PlayDialogue(dialogueLinesToPlay,
            () => Chimp_AfterDialogueAction(player, itemToGive));
    }

    private void Chimp_AfterDialogueAction(
        PlayerController player, ItemStack? itemToGive)
    {
        if (itemToGive != null)
        {
            player.GetInventory().AddItem(itemToGive.Value);

            chimpItemInWorld.Hide();

            LastGiveTime = Time.time;
            TimeBetweenGives = Random.Range(
                timeBetweenGivesRange.x, timeBetweenGivesRange.y);
        }

        chimpSpinner.StartSpinning();
    }

    public bool ItemGiveAvailable() => LastGiveTime + TimeBetweenGives <= Time.time;
}
