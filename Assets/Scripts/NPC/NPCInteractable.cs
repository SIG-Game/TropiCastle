using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : Interactable
{
    [SerializeField] protected Sprite front, back, left, right;
    [SerializeField] protected Sprite defaultSprite;
    [SerializeField] protected List<string> dialogueLines;

    protected SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Interact(PlayerController player)
    {
        Interact(player, () => spriteRenderer.sprite = defaultSprite);
    }

    protected void Interact(PlayerController player, Action afterDialogueAction)
    {
        FacePlayer(player);

        DialogueBox.Instance.PlayDialogue(dialogueLines, afterDialogueAction);
    }

    public void FacePlayer(PlayerController player)
    {
        switch (player.lastDirection)
        {
            case PlayerController.Direction.Up:
                spriteRenderer.sprite = front;
                break;
            case PlayerController.Direction.Down:
                spriteRenderer.sprite = back;
                break;
            case PlayerController.Direction.Left:
                spriteRenderer.sprite = right;
                break;
            case PlayerController.Direction.Right:
                spriteRenderer.sprite = left;
                break;
        }
    }
}
