using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBoxUI;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject characterNameUI;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float characterScrollWaitSeconds;

    private IEnumerator<string> linesEnumerator;
    private Action afterDialogueAction;
    private Coroutine displayScrollingTextCoroutine;
    private WaitForSeconds characterScrollWaitForSeconds;
    private bool textScrolling;

    public static DialogueBox Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        linesEnumerator = Enumerable.Empty<string>().GetEnumerator();
        characterScrollWaitForSeconds = new WaitForSeconds(characterScrollWaitSeconds);
        textScrolling = false;
    }

    // Must run before PlayerController Update method to prevent
    // dialogue from immediately advancing after interaction
    private void Update()
    {
        bool canAdvanceDialogue = DialogueBoxOpen() && !PauseController.Instance.GamePaused;
        if (canAdvanceDialogue)
        {
            // Get both inputs so that neither can be used elsewhere
            bool useItemButtonInput =
                inputManager.GetUseItemButtonDownIfUnusedThisFrame();
            bool interactButtonInput =
                inputManager.GetInteractButtonDownIfUnusedThisFrame();

            bool advanceDialogueInputPressedThisFrame =
                useItemButtonInput || interactButtonInput;
            if (advanceDialogueInputPressedThisFrame)
            {
                AdvanceDialogue();
            }
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void AdvanceDialogue()
    {
        if (textScrolling)
        {
            SkipTextScrolling();
        }
        else if (linesEnumerator.MoveNext())
        {
            DisplayScrollingText(linesEnumerator.Current);
        }
        else
        {
            CloseDialogueBox();
        }
    }

    public void PlayDialogue(string line, Action afterDialogueAction = null)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            Debug.LogWarning($"Dialogue line is empty");
            return;
        }

        linesEnumerator = Enumerable.Empty<string>().GetEnumerator();

        this.afterDialogueAction = afterDialogueAction;

        PlayFirstDialogueLine(line);
    }

    public void PlayDialogue(IEnumerable<string> lines, Action afterDialogueAction = null)
    {
        linesEnumerator = lines.GetEnumerator();

        if (!linesEnumerator.MoveNext())
        {
            Debug.LogWarning($"Dialogue lines {nameof(IEnumerable<string>)} is empty");
            return;
        }

        this.afterDialogueAction = afterDialogueAction;

        PlayFirstDialogueLine(linesEnumerator.Current);
    }

    public void SetCharacterName(string characterName)
    {
        characterNameText.text = characterName;

        characterNameUI.SetActive(true);
    }

    private void PlayFirstDialogueLine(string line)
    {
        DisplayScrollingText(line);
        dialogueBoxUI.SetActive(true);
    }

    private void DisplayScrollingText(string text)
    {
        StopDisplayScrollingTextCoroutine();
        displayScrollingTextCoroutine = StartCoroutine(DisplayScrollingTextCoroutine(text));
    }

    private IEnumerator DisplayScrollingTextCoroutine(string text)
    {
        textScrolling = true;

        dialogueText.text = text;

        for (int visibleCharacters = 0; visibleCharacters < text.Length; ++visibleCharacters)
        {
            dialogueText.maxVisibleCharacters = visibleCharacters;

            yield return characterScrollWaitForSeconds;
        }

        dialogueText.maxVisibleCharacters = text.Length;

        textScrolling = false;
    }

    private void SkipTextScrolling()
    {
        StopDisplayScrollingTextCoroutine();
        textScrolling = false;
        dialogueText.maxVisibleCharacters = dialogueText.text.Length;
    }

    private void CloseDialogueBox()
    {
        afterDialogueAction?.Invoke();
        dialogueBoxUI.SetActive(false);
        characterNameUI.SetActive(false);
    }

    private void StopDisplayScrollingTextCoroutine()
    {
        if (displayScrollingTextCoroutine != null)
        {
            StopCoroutine(displayScrollingTextCoroutine);
        }
    }

    private bool DialogueBoxOpen() => dialogueBoxUI.activeInHierarchy;
}
