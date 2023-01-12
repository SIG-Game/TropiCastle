using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chimp : NPCInteractable
{
    private List<Sprite> directionSprites;
    private ItemScriptableObject appleItemScriptableObject;
    private Coroutine spinCoroutine;

    private void Start()
    {
        directionSprites = new List<Sprite> { front, left, back, right };
        appleItemScriptableObject = Resources.Load<ItemScriptableObject>("Items/Apple");

        spinCoroutine = StartCoroutine(Spin());
    }

    public override void Interact(PlayerController player)
    {
        StopCoroutine(spinCoroutine);

        Interact(player, () => Chimp_AfterDialogueAction(player));
    }

    private IEnumerator Spin()
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
        player.GetInventory().AddItem(appleItemScriptableObject, 1);
        spinCoroutine = StartCoroutine(Spin());
    }
}
