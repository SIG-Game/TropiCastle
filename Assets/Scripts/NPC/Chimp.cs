﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chimp : NPCInteractable
{
    List<Sprite> directionSprites;
    ItemScriptableObject appleItemScriptableObject;

    void Start()
    {
        directionSprites = new List<Sprite> { front, left, back, right };
        appleItemScriptableObject = Resources.Load<ItemScriptableObject>("Items/Apple");

        StartCoroutine("Spin");
    }

    public override void Interact(PlayerController player)
    {
        this.player = player;

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
        player.GetInventory().AddItem(appleItemScriptableObject, 1);
        StartCoroutine("Spin");
    }
}
