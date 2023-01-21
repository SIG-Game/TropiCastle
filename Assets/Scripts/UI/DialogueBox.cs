﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBoxUI;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private PlayerController player;

    private IEnumerator<string> linesEnumerator;
    private Action afterDialogueAction;

    public static DialogueBox Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        linesEnumerator = Enumerable.Empty<string>().GetEnumerator();
        afterDialogueAction = null;
    }

    private void Update()
    {
        // Must run before PlayerController to prevent dialogue from immediately advancing
        if ((Input.GetButtonDown("Interact") || Input.GetMouseButtonDown(0)) &&
            dialogueBoxUI.activeInHierarchy && !PauseController.Instance.GamePaused)
        {
            if (linesEnumerator.MoveNext())
            {
                dialogueText.text = linesEnumerator.Current;
            }
            else
            {
                afterDialogueAction?.Invoke(); // afterDialogueAction may be null

                dialogueBoxUI.SetActive(false);
                player.dialogueBoxOpen = false;
            }
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void PlayDialogue(IEnumerable<string> lines, Action afterDialogueAction = null)
    {
        this.afterDialogueAction = afterDialogueAction;

        linesEnumerator = lines.GetEnumerator();

        if (!linesEnumerator.MoveNext())
        {
            Debug.LogWarning($"Dialogue lines {nameof(IEnumerable<string>)} is empty");
            return;
        }

        dialogueText.text = linesEnumerator.Current;
        dialogueBoxUI.SetActive(true);
        player.dialogueBoxOpen = true;
        player.canUseDialogueInputs = false;
    }
}
