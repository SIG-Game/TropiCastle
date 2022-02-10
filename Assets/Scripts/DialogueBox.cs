using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public static DialogueBox Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject dialogueBoxUI;
    public TextMeshProUGUI dialogueText;
    public PlayerController player;
    public Action AfterDialogueAction;

    private List<string>.Enumerator linesEnumerator;

    public void PlayDialogue(List<string> lines)
    {
        if (lines.Count == 0)
        {
            Debug.LogWarning("Dialogue lines list is empty");
            return;
        }

        linesEnumerator = lines.GetEnumerator();
        linesEnumerator.MoveNext();

        dialogueText.text = linesEnumerator.Current;
        dialogueBoxUI.SetActive(true);
        player.dialogueBoxOpen = true;
        player.canInteract = false;
    }

    private void Update()
    {
        // Must run before PlayerController to prevent dialogue from immediately advancing
        if (Input.GetButtonDown("Interact") && dialogueBoxUI.activeInHierarchy)
        {
            if (linesEnumerator.MoveNext())
            {
                dialogueText.text = linesEnumerator.Current;
            }
            else
            {
                AfterDialogueAction?.Invoke(); // Action may be null
                AfterDialogueAction = null; // Action will be set again if used

                dialogueBoxUI.SetActive(false);
                player.dialogueBoxOpen = false;
            }
        }
    }
}
