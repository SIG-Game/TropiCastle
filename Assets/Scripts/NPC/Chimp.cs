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
    [SerializeField] private Transform itemToGiveInWorld;
    [SerializeField] private List<Vector3> itemToGiveInWorldOffsets;

    private List<Sprite> directionSprites;
    private Coroutine spinCoroutine;
    private SpriteRenderer itemToGiveInWorldSpriteRenderer;
    private ItemWithAmount itemToGive;
    private float lastGiveTimeSeconds;
    private float timeSecondsUntilNextGive;

    private void Start()
    {
        directionSprites = new List<Sprite> { front, left, back, right };

        spinCoroutine = StartCoroutine(SpinCoroutine());

        itemToGiveInWorldSpriteRenderer = itemToGiveInWorld.GetComponent<SpriteRenderer>();

        lastGiveTimeSeconds = 0f;
        timeSecondsUntilNextGive = 0f;
    }

    public override void Interact(PlayerController player)
    {
        StopCoroutine(spinCoroutine);

        FacePlayer(player);

        bool giveItem = lastGiveTimeSeconds + timeSecondsUntilNextGive <= Time.time;
        if (giveItem)
        {
            int itemToGiveIndex = Random.Range(0, potentialItemsToGive.Count);
            itemToGive = potentialItemsToGive[itemToGiveIndex];
            dialogueLines[1] = itemToGiveDialogueLines[itemToGiveIndex];
            ShowItemToGiveInWorld();
        }
        else
        {
            dialogueLines[1] = notGivingItemDialogueLine;
        }

        DialogueBox.Instance.PlayDialogue(dialogueLines,
            () => Chimp_AfterDialogueAction(player, giveItem));
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

            HideItemToGiveInWorld();

            lastGiveTimeSeconds = Time.time;
            timeSecondsUntilNextGive = GetRandomTimeSecondsUntilNextGive();
        }

        spinCoroutine = StartCoroutine(SpinCoroutine());
    }

    private void ShowItemToGiveInWorld()
    {
        Vector3 itemToGiveInWorldOffset;

        if (spriteRenderer.sprite == front)
            itemToGiveInWorldOffset = itemToGiveInWorldOffsets[0];
        else if (spriteRenderer.sprite == back)
            itemToGiveInWorldOffset = itemToGiveInWorldOffsets[1];
        else if (spriteRenderer.sprite == left)
            itemToGiveInWorldOffset = itemToGiveInWorldOffsets[2];
        else
            itemToGiveInWorldOffset = itemToGiveInWorldOffsets[3];

        if (spriteRenderer.sprite == back)
            itemToGiveInWorldSpriteRenderer.sortingOrder = -1;
        else
            itemToGiveInWorldSpriteRenderer.sortingOrder = 1;

        itemToGiveInWorld.position = transform.position + itemToGiveInWorldOffset;
        itemToGiveInWorldSpriteRenderer.sprite = itemToGive.itemData.sprite;
    }

    private void HideItemToGiveInWorld()
    {
        itemToGiveInWorldSpriteRenderer.sprite = null;
    }

    private float GetRandomTimeSecondsUntilNextGive() =>
        Random.Range(minTimeSecondsBetweenGives, maxTimeSecondsBetweenGives);
}
