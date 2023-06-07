using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : Interactable
{
    [SerializeField] protected List<string> dialogueLines;

    protected CharacterDirectionController directionController;

    protected virtual void Awake()
    {
        directionController = GetComponent<CharacterDirectionController>();
    }

    public override void Interact(PlayerController player)
    {
        FacePlayer(player);

        Action afterDialogueAction = directionController.UseDefaultDirection;
        DialogueBox.Instance.PlayDialogue(dialogueLines, afterDialogueAction);
    }

    public void FacePlayer(PlayerController player)
    {
        CharacterDirection newDirection = player.Direction switch
        {
            CharacterDirection.Up => CharacterDirection.Down,
            CharacterDirection.Down => CharacterDirection.Up,
            CharacterDirection.Left => CharacterDirection.Right,
            CharacterDirection.Right => CharacterDirection.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(player.Direction))
        };

        directionController.Direction = newDirection;
    }
}
