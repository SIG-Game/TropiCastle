using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chimp : NPCInteractable
{
    [SerializeField] private List<string> givingItemDialogueLines;
    [SerializeField] private List<string> notGivingItemDialogueLines;
    [SerializeField] private List<string> playerInventoryFullDialogueLines;
    [SerializeField] private NPCItemOfferingScriptableObject itemOffering;
    [SerializeField] private Vector2 timeBetweenGivesSecondsRange;
    [SerializeField] private NPCSpinner chimpSpinner;
    [SerializeField] private CharacterItemInWorldController chimpItemInWorld;
    [SerializeField] private DialogueBox dialogueBox;

    public float LastGiveTimeSeconds { get; set; }
    public float TimeBetweenGivesSeconds { get; set; }

    private WeightedRandomSelector itemSelector;

    private const string chimpCharacterName = "Chimp";

    protected override void Awake()
    {
        base.Awake();

        List<float> itemWeights = itemOffering.PotentialItemsToGive
            .Select(x => x.ProbabilityWeight).ToList();

        itemSelector = new WeightedRandomSelector(itemWeights);

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
            int itemToGiveIndex = itemSelector.SelectIndex();
            itemToGive = itemOffering.PotentialItemsToGive[itemToGiveIndex].Item;

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
