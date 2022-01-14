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

    public void PlayDialogue(string dialogue)
    {
        dialogueText.text = dialogue;
        dialogueBoxUI.SetActive(true);
        player.dialogueBoxOpen = true;
        player.canInteract = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && dialogueBoxUI.activeInHierarchy)
        {
            dialogueBoxUI.SetActive(false);
            player.dialogueBoxOpen = false;
        }
    }
}
