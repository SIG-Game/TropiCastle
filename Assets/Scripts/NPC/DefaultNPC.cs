using System;
using System.Collections.Generic;
using UnityEngine;

public class DefaultNPC : NPCInteractable
{
    [SerializeField] private List<string> dialogueLines;

    public override void Interact(PlayerController player)
    {
        FacePlayer(player);

        Action afterDialogueAction = directionController.UseDefaultDirection;
        DialogueBox.Instance.PlayDialogue(dialogueLines, afterDialogueAction);
    }
}
