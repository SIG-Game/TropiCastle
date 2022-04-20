using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chimp : NPCInteractable
{
    List<Sprite> directionSprites;

    void Start()
    {
        directionSprites = new List<Sprite> { front, left, back, right };

        StartCoroutine("Spin");
    }

    public override void Interact()
    {
        StopCoroutine("Spin");

        FacePlayer();

        DialogueBox.Instance.AfterDialogueAction = Chimp_AfterDialogueAction;
        DialogueBox.Instance.PlayDialogue(dialogueLines);
    }

    IEnumerator Spin()
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

    private void Chimp_AfterDialogueAction()
    {
        player.GetInventory().AddItem(Item.ItemType.Apple, 1);
        StartCoroutine("Spin");
    }
}
