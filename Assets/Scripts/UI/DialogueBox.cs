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
    [SerializeField] private PlayerController player;
    [SerializeField] private float characterScrollWaitSeconds;

    private const string alphaColorTag = "<color=#00000000>";

    private IEnumerator<string> linesEnumerator;
    private Action afterDialogueAction;
    private Coroutine displayScrollingTextCoroutineObject;
    private WaitForSeconds characterScrollWaitForSecondsObject;
    private bool textScrolling;

    public static DialogueBox Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        linesEnumerator = Enumerable.Empty<string>().GetEnumerator();
        afterDialogueAction = null;
        characterScrollWaitForSecondsObject = new WaitForSeconds(characterScrollWaitSeconds);
        textScrolling = false;
    }

    private void Update()
    {
        // Must run before PlayerController to prevent dialogue from immediately advancing
        if (DialogueBoxOpen() && !PauseController.Instance.GamePaused)
        {
            // Get both inputs so that neither can be used elsewhere
            bool leftClickInput = InputManager.Instance.GetLeftClickDownIfUnusedThisFrame();
            bool interactButtonInput = InputManager.Instance.GetInteractButtonDownIfUnusedThisFrame();

            if (leftClickInput || interactButtonInput)
            {
                if (textScrolling)
                {
                    StopDisplayScrollingTextCoroutineIfNotNull();
                    textScrolling = false;
                    dialogueText.text = linesEnumerator.Current;
                }
                else if (linesEnumerator.MoveNext())
                {
                    DisplayScrollingText(linesEnumerator.Current);
                }
                else
                {
                    afterDialogueAction?.Invoke(); // afterDialogueAction may be null

                    dialogueBoxUI.SetActive(false);
                }
            }
        }
    }

    private void OnDestroy()
    {
        Instance = null;
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

        DisplayScrollingText(line);
        dialogueBoxUI.SetActive(true);
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

        DisplayScrollingText(linesEnumerator.Current);
        dialogueBoxUI.SetActive(true);
    }

    private void DisplayScrollingText(string text)
    {
        StopDisplayScrollingTextCoroutineIfNotNull();
        displayScrollingTextCoroutineObject = StartCoroutine(DisplayScrollingTextCoroutine(text));
    }

    private IEnumerator DisplayScrollingTextCoroutine(string text)
    {
        textScrolling = true;
        int alphaTagIndex = 0;

        while (alphaTagIndex < text.Length)
        {
            string scrollingText = text.Insert(alphaTagIndex, alphaColorTag);
            dialogueText.text = scrollingText;

            yield return characterScrollWaitForSecondsObject;

            alphaTagIndex++;
        }

        dialogueText.text = text;
        textScrolling = false;
    }

    private void StopDisplayScrollingTextCoroutineIfNotNull()
    {
        if (displayScrollingTextCoroutineObject != null)
        {
            StopCoroutine(displayScrollingTextCoroutineObject);
        }
    }

    public bool DialogueBoxOpen()
    {
        return dialogueBoxUI.activeInHierarchy;
    }
}
