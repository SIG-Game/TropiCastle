using System.Collections.Generic;
using UnityEngine;

public class DialogueNPC : NPCInteractable
{
    [SerializeField] private List<string> dialogueLines;
    [SerializeField] private DialogueBox dialogueBox;

    public override void Interact(PlayerController player)
    {
        FacePlayer(player);

        dialogueBox.PlayDialogue(dialogueLines,
            directionController.UseDefaultDirection);
    }
}
