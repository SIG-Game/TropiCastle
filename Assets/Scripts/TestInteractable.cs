using UnityEngine;

public class TestInteractable : Interactable
{
    public PlayerController player;
    public Sprite front, back, left, right;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Interact()
    {
        Debug.Log("Interacted");
        
        switch (player.getLastDir())
        {
            case PlayerController.Direction.UP:
                spriteRenderer.sprite = front;
                break;
            case PlayerController.Direction.DOWN:
                spriteRenderer.sprite = back;
                break;
            case PlayerController.Direction.LEFT:
                spriteRenderer.sprite = right;
                break;
            case PlayerController.Direction.RIGHT:
                spriteRenderer.sprite = left;
                break;
        }

        DialogueBox.Instance.PlayDialogue("Hello.");
    }
}
