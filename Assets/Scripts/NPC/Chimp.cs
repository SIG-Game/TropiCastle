using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chimp : NPCInteractable
{
    [SerializeField] private List<string> givingItemDialogueLines;
    [SerializeField] private List<string> notGivingItemDialogueLines;
    [SerializeField] private List<string> playerInventoryFullDialogueLines;
    [SerializeField] private Vector2 timeBetweenGivesRange;
    [SerializeField] private NPCItemOfferingSelector itemOfferingSelector;
    [SerializeField] private NPCSpinner chimpSpinner;
    [SerializeField] private CharacterItemInWorldController chimpItemInWorld;
    [SerializeField] private DialogueBox dialogueBox;

    public float LastGiveTime { get; set; }
    public float TimeBetweenGives { get; set; }

    private const string chimpCharacterName = "Chimp";

    protected override void Awake()
    {
        base.Awake();

        LastGiveTime = 0f;
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

        ItemStack itemToGive = null;
        List<string> dialogueLinesToPlay = null;

        if (ItemGiveAvailable())
        {
            itemToGive = itemOfferingSelector.SelectItemToGive();

            if (!player.GetInventory().CanAddItem(itemToGive))
            {
                dialogueLinesToPlay = playerInventoryFullDialogueLines;
                itemToGive = null;
            }
            else
            {
                dialogueLinesToPlay = givingItemDialogueLines;
                chimpItemInWorld.ShowItem(itemToGive);
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

    private void Chimp_AfterDialogueAction(PlayerController player, ItemStack itemToGive)
    {
        if (itemToGive != null)
        {
            player.GetInventory().AddItem(itemToGive);

            chimpItemInWorld.Hide();

            LastGiveTime = Time.time;
            TimeBetweenGives = GetRandomTimeBetweenGives();
        }

        chimpSpinner.StartSpinning();
    }

    public bool ItemGiveAvailable() => LastGiveTime + TimeBetweenGives <= Time.time;

    private float GetRandomTimeBetweenGives() =>
        Random.Range(timeBetweenGivesRange.x, timeBetweenGivesRange.y);
}
