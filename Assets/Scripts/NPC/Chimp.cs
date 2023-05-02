using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chimp : NPCInteractable
{
    [SerializeField] private List<ItemWithAmount> potentialItemsToGive;
    [SerializeField] private List<string> itemToGiveDialogueLines;
    [SerializeField] private string notGivingItemDialogueLine;
    [SerializeField] private string playerInventoryFullDialogueLine;
    [SerializeField] private Vector2 timeBetweenGivesSecondsRange;
    [SerializeField] private Vector2Int spinsBeforeWaitRange;
    [SerializeField] private Vector2 timeBetweenSpinsSecondsRange;
    [SerializeField] private Transform itemToGiveInWorld;

    // Direction order for this variable is up, down, left, right
    [SerializeField] private List<Vector3> itemToGiveInWorldOffsets;

    private List<CharacterDirection> spinDirections;
    private Coroutine spinCoroutine;
    private SpriteRenderer itemToGiveInWorldSpriteRenderer;
    private ItemWithAmount itemToGive;
    private float lastGiveTimeSeconds;
    private float timeSecondsUntilNextGive;

    private void Start()
    {
        spinDirections = new List<CharacterDirection> { CharacterDirection.Down,
            CharacterDirection.Left, CharacterDirection.Up, CharacterDirection.Right };

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
            if (player.GetInventory().IsFull())
            {
                dialogueLines[1] = playerInventoryFullDialogueLine;
                giveItem = false;
            }
            else
            {
                int itemToGiveIndex = Random.Range(0, potentialItemsToGive.Count);
                itemToGive = potentialItemsToGive[itemToGiveIndex];
                dialogueLines[1] = itemToGiveDialogueLines[itemToGiveIndex];
                ShowItemToGiveInWorld();
            }
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
        Vector3 itemToGiveInWorldOffset =
            itemToGiveInWorldOffsets[(int)directionController.Direction];

        if (directionController.Direction == CharacterDirection.Up)
            itemToGiveInWorldSpriteRenderer.sortingOrder = -1;
        else
            itemToGiveInWorldSpriteRenderer.sortingOrder = 1;

        itemToGiveInWorld.localPosition = itemToGiveInWorldOffset;
        itemToGiveInWorldSpriteRenderer.sprite = itemToGive.itemData.sprite;
    }

    private void HideItemToGiveInWorld()
    {
        itemToGiveInWorldSpriteRenderer.sprite = null;
    }

    private float GetRandomTimeSecondsUntilNextGive() =>
        Random.Range(timeBetweenGivesSecondsRange.x, timeBetweenGivesSecondsRange.y);
}
