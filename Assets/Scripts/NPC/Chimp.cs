using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chimp : NPCInteractable
{
    [SerializeField] private List<string> notGivingItemDialogueLines;
    [SerializeField] private List<string> playerInventoryFullDialogueLines;
    [SerializeField] private List<ItemWithAmount> potentialItemsToGive;
    [SerializeField] private Vector2 timeBetweenGivesSecondsRange;
    [SerializeField] private Vector2Int spinsBeforeWaitRange;
    [SerializeField] private Vector2 timeBetweenSpinsSecondsRange;
    [SerializeField] private CharacterItemInWorldController chimpItemInWorld;

    private List<CharacterDirection> spinDirections;
    private Coroutine spinCoroutine;
    private float lastGiveTimeSeconds;
    private float timeBetweenGivesSeconds;

    protected override void Awake()
    {
        base.Awake();

        spinDirections = new List<CharacterDirection> { CharacterDirection.Down,
            CharacterDirection.Left, CharacterDirection.Up, CharacterDirection.Right };

        lastGiveTimeSeconds = 0f;
        timeBetweenGivesSeconds = 0f;
    }

    private void Start()
    {
        spinCoroutine = StartCoroutine(SpinCoroutine());
    }

    public override void Interact(PlayerController player)
    {
        StopCoroutine(spinCoroutine);

        FacePlayer(player);

        ItemWithAmount itemToGive = null;
        List<string> dialogueLinesToPlay = null;

        if (ItemGiveAvailable())
        {
            int itemToGiveIndex = Random.Range(0, potentialItemsToGive.Count);
            itemToGive = potentialItemsToGive[itemToGiveIndex];

            if (!player.GetInventory().CanAddItem(itemToGive))
            {
                dialogueLinesToPlay = playerInventoryFullDialogueLines;
                itemToGive = null;
            }
            else
            {
                dialogueLinesToPlay = dialogueLines;
                chimpItemInWorld.ShowCharacterItemInWorld(itemToGive.itemData.sprite);
            }
        }
        else
        {
            dialogueLinesToPlay = notGivingItemDialogueLines;
        }

        DialogueBox.Instance.PlayDialogue(dialogueLinesToPlay,
            () => Chimp_AfterDialogueAction(player, itemToGive));
    }

    private IEnumerator SpinCoroutine()
    {
        while (true)
        {
            int numberOfSpinsBeforeWait = Random.Range(spinsBeforeWaitRange.x,
                spinsBeforeWaitRange.y + 1);

            for (int i = 0; i < numberOfSpinsBeforeWait; ++i)
            {
                foreach (CharacterDirection direction in spinDirections)
                {
                    directionController.Direction = direction;
                    yield return new WaitForSeconds(0.175f);
                }
            }

            directionController.Direction = CharacterDirection.Down;

            float waitBeforeSpinningSeconds = Random.Range(timeBetweenSpinsSecondsRange.x,
                timeBetweenSpinsSecondsRange.y);

            yield return new WaitForSeconds(waitBeforeSpinningSeconds);
        }
    }

    private void Chimp_AfterDialogueAction(PlayerController player, ItemWithAmount itemToGive)
    {
        if (itemToGive != null)
        {
            player.GetInventory().AddItem(itemToGive);

            chimpItemInWorld.HideCharacterItemInWorld();

            lastGiveTimeSeconds = Time.time;
            timeBetweenGivesSeconds = GetRandomTimeBetweenGivesSeconds();
        }

        spinCoroutine = StartCoroutine(SpinCoroutine());
    }

    private float GetRandomTimeBetweenGivesSeconds() =>
        Random.Range(timeBetweenGivesSecondsRange.x, timeBetweenGivesSecondsRange.y);

    private bool ItemGiveAvailable() =>
        lastGiveTimeSeconds + timeBetweenGivesSeconds <= Time.time;

    public SerializableChimpState GetSerializableChimpState()
    {
        float timeUntilNextGiveSeconds;

        if (ItemGiveAvailable())
        {
            timeUntilNextGiveSeconds = 0f;
        }
        else
        {
            float nextGiveTimeSeconds = lastGiveTimeSeconds + timeBetweenGivesSeconds;

            timeUntilNextGiveSeconds = nextGiveTimeSeconds - Time.time;
        }

        var serializableChimpState = new SerializableChimpState
        {
            TimeSecondsUntilNextGive = timeUntilNextGiveSeconds
        };

        return serializableChimpState;
    }

    public void SetPropertiesFromSerializableChimpState(
        SerializableChimpState serializableChimpState)
    {
        timeBetweenGivesSeconds = serializableChimpState.TimeSecondsUntilNextGive;
    }

    [Serializable]
    public class SerializableChimpState
    {
        public float TimeSecondsUntilNextGive;
    }
}
