using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chimp : NPCInteractable
{
    [SerializeField] private List<ItemWithAmount> potentialItemsToGive;
    [SerializeField] private List<string> itemToGiveDialogueLines;
    [SerializeField] private string notGivingItemDialogueLine;
    [SerializeField] private float minTimeSecondsBetweenGives;
    [SerializeField] private float maxTimeSecondsBetweenGives;

    private List<Sprite> directionSprites;
    private Coroutine spinCoroutine;
    private ItemWithAmount itemToGive;
    private float lastGiveTimeSeconds;
    private float timeSecondsUntilNextGive;

    private void Start()
    {
        directionSprites = new List<Sprite> { front, left, back, right };

        spinCoroutine = StartCoroutine(SpinCoroutine());

        lastGiveTimeSeconds = 0f;
        timeSecondsUntilNextGive = 0f;
    }

    public override void Interact(PlayerController player)
    {
        StopCoroutine(spinCoroutine);

        bool giveItem = lastGiveTimeSeconds + timeSecondsUntilNextGive <= Time.time;

        if (giveItem)
        {
            int itemToGiveIndex = Random.Range(0, potentialItemsToGive.Count);
            itemToGive = potentialItemsToGive[itemToGiveIndex];
            dialogueLines[1] = itemToGiveDialogueLines[itemToGiveIndex];
        }
        else
        {
            dialogueLines[1] = notGivingItemDialogueLine;
        }

        Interact(player, () => Chimp_AfterDialogueAction(player, giveItem));
    }

    private IEnumerator SpinCoroutine()
    {
        while (true)
        {
            foreach (Sprite directionSprite in directionSprites)
            {
                spriteRenderer.sprite = directionSprite;
                yield return new WaitForSeconds(0.175f);
            }
        }
    }

    private void Chimp_AfterDialogueAction(PlayerController player, bool giveItem)
    {
        if (giveItem)
        {
            player.GetInventory().AddItem(itemToGive);

            lastGiveTimeSeconds = Time.time;
            timeSecondsUntilNextGive = GetRandomTimeSecondsUntilNextGive();
        }

        spinCoroutine = StartCoroutine(SpinCoroutine());
    }

    private float GetRandomTimeSecondsUntilNextGive() =>
        Random.Range(minTimeSecondsBetweenGives, maxTimeSecondsBetweenGives);
}
