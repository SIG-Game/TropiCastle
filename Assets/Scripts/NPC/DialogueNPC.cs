using System.Collections.Generic;
using UnityEngine;

public class DialogueNPC : NPCInteractable
{
    [SerializeField] private List<string> dialogueLines;

    [Inject] private DialogueBox dialogueBox;
    [Inject] private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();
    }

    public override void Interact()
    {
        FacePlayer(playerController);

        dialogueBox.PlayDialogue(dialogueLines,
            StartWaitThenReturnToDefaultDirectionCouroutine);
    }
}
