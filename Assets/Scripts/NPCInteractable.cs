﻿using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : Interactable
{
    public PlayerController player;
    public Sprite front, back, left, right;
    public List<string> dialogueLines;

    protected SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Interact()
    {
        Debug.Log("Interacted");

        FacePlayer();

        DialogueBox.Instance.PlayDialogue(dialogueLines);
    }

    public void FacePlayer()
    {
        switch (player.getLastDir())
        {
            case PlayerController.Direction.UP:
                spriteRenderer.sprite = front;
                break;
            case PlayerController.Direction.DOWN:
                spriteRenderer.sprite = back;
                break;
            case PlayerController.Direction.LEFT:
                spriteRenderer.sprite = right;
                break;
            case PlayerController.Direction.RIGHT:
                spriteRenderer.sprite = left;
                break;
        }
    }
}
