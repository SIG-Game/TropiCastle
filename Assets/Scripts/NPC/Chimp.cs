using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chimp : NPCInteractable
{
    [SerializeField] private List<string> givingItemDialogueLines;
    [SerializeField] private List<string> notGivingItemDialogueLines;
    [SerializeField] private List<string> playerInventoryFullDialogueLines;
    [SerializeField] private Vector2 timeBetweenGivesSecondsRange;
    [SerializeField] private NPCItemOfferingSelector itemOfferingSelector;
    [SerializeField] private NPCSpinner chimpSpinner;
    [SerializeField] private CharacterItemInWorldController chimpItemInWorld;
    [SerializeField] private DialogueBox dialogueBox;

    public float LastGiveTimeSeconds { get; set; }
    public float TimeBetweenGivesSeconds { get; set; }

    private const string chimpCharacterName = "Chimp";

    protected override void Awake()
    {
        base.Awake();

        LastGiveTimeSeconds = 0f;
        TimeBetweenGivesSeconds = 0f;
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

            LastGiveTimeSeconds = Time.time;
            TimeBetweenGivesSeconds = GetRandomTimeBetweenGivesSeconds();
        }

        chimpSpinner.StartSpinning();
    }

    public bool ItemGiveAvailable() =>
        LastGiveTimeSeconds + TimeBetweenGivesSeconds <= Time.time;

    private float GetRandomTimeBetweenGivesSeconds() =>
        Random.Range(timeBetweenGivesSecondsRange.x, timeBetweenGivesSecondsRange.y);
}
