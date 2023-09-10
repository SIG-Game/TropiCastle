﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chimp : NPCInteractable, ISavable<Chimp.SerializableChimpState>
{
    [SerializeField] private List<string> givingItemDialogueLines;
    [SerializeField] private List<string> notGivingItemDialogueLines;
    [SerializeField] private List<string> playerInventoryFullDialogueLines;
    [SerializeField] private List<ItemWithAmount> potentialItemsToGive;
    [SerializeField] private Vector2 timeBetweenGivesSecondsRange;
    [SerializeField] private NPCSpinner chimpSpinner;
    [SerializeField] private CharacterItemInWorldController chimpItemInWorld;

    private float lastGiveTimeSeconds;
    private float timeBetweenGivesSeconds;

    private const string chimpCharacterName = "Chimp";

    protected override void Awake()
    {
        base.Awake();

        lastGiveTimeSeconds = 0f;
        timeBetweenGivesSeconds = 0f;
    }

    private void Start()
    {
        chimpSpinner.StartSpinning();
    }

    public override void Interact(PlayerController player)
    {
        chimpSpinner.StopSpinning();

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
                dialogueLinesToPlay = givingItemDialogueLines;
                chimpItemInWorld.ShowCharacterItemInWorld(itemToGive);
            }
        }
        else
        {
            dialogueLinesToPlay = notGivingItemDialogueLines;
        }

        DialogueBox.Instance.SetCharacterName(chimpCharacterName);
        DialogueBox.Instance.PlayDialogue(dialogueLinesToPlay,
            () => Chimp_AfterDialogueAction(player, itemToGive));
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

        chimpSpinner.StartSpinning();
    }

    private float GetRandomTimeBetweenGivesSeconds() =>
        Random.Range(timeBetweenGivesSecondsRange.x, timeBetweenGivesSecondsRange.y);

    private bool ItemGiveAvailable() =>
        lastGiveTimeSeconds + timeBetweenGivesSeconds <= Time.time;

    public SerializableChimpState GetSerializableState()
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

        var serializableState = new SerializableChimpState
        {
            TimeSecondsUntilNextGive = timeUntilNextGiveSeconds
        };

        return serializableState;
    }

    public void SetPropertiesFromSerializableState(
        SerializableChimpState serializableState)
    {
        timeBetweenGivesSeconds = serializableState.TimeSecondsUntilNextGive;

        lastGiveTimeSeconds = Time.time;
    }

    [Serializable]
    public class SerializableChimpState
    {
        public float TimeSecondsUntilNextGive;
    }
}
