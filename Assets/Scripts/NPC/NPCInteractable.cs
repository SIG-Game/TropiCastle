using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : Interactable
{
    [SerializeField] protected Sprite front, back, left, right;
    [SerializeField] private CharacterDirection defaultDirection;
    [SerializeField] protected List<string> dialogueLines;

    protected CharacterDirection currentDirection;
    protected SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetDirectionAndUpdateSprite(defaultDirection);
    }

    public override void Interact(PlayerController player)
    {
        FacePlayer(player);

        Action afterDialogueAction = () => SetDirectionAndUpdateSprite(defaultDirection);
        DialogueBox.Instance.PlayDialogue(dialogueLines, afterDialogueAction);
    }

    public void FacePlayer(PlayerController player)
    {
        CharacterDirection newDirection = player.LastDirection switch
        {
            CharacterDirection.Up => CharacterDirection.Down,
            CharacterDirection.Down => CharacterDirection.Up,
            CharacterDirection.Left => CharacterDirection.Right,
            CharacterDirection.Right => CharacterDirection.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(player.LastDirection))
        };

        SetDirectionAndUpdateSprite(newDirection);
    }

    protected void SetDirectionAndUpdateSprite(CharacterDirection direction)
    {
        currentDirection = direction;

        spriteRenderer.sprite = currentDirection switch
        {
            CharacterDirection.Up => back,
            CharacterDirection.Down => front,
            CharacterDirection.Left => left,
            CharacterDirection.Right => right,
            _ => throw new ArgumentOutOfRangeException(nameof(currentDirection))
        };
    }
}
