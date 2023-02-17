using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chimp : NPCInteractable
{
    [SerializeField] private List<ItemWithAmount> potentialItemsToGive;
    [SerializeField] private List<string> itemToGiveDialogueLines;

    private List<Sprite> directionSprites;
    private Coroutine spinCoroutine;
    private ItemWithAmount itemToGive;

    private void Start()
    {
        directionSprites = new List<Sprite> { front, left, back, right };

        spinCoroutine = StartCoroutine(SpinCoroutine());
    }

    public override void Interact(PlayerController player)
    {
        StopCoroutine(spinCoroutine);

        int itemToGiveIndex = Random.Range(0, potentialItemsToGive.Count);
        itemToGive = potentialItemsToGive[itemToGiveIndex];
        dialogueLines[1] = itemToGiveDialogueLines[itemToGiveIndex];

        Interact(player, () => Chimp_AfterDialogueAction(player));
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

    private void Chimp_AfterDialogueAction(PlayerController player)
    {
        player.GetInventory().AddItem(itemToGive);
        spinCoroutine = StartCoroutine(SpinCoroutine());
    }
}
