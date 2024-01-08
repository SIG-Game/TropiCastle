using UnityEngine;

public abstract class NPCInteractable : MonoBehaviour, IInteractable
{
    protected CharacterDirectionController directionController;

    protected virtual void Awake()
    {
        directionController = GetComponent<CharacterDirectionController>();
    }

    public void FacePlayer(PlayerController player) =>
        directionController.UseOppositeOfDirection(player.Direction);

    public abstract void Interact();
}
