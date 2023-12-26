public abstract class NPCInteractable : Interactable
{
    protected CharacterDirectionController directionController;

    protected virtual void Awake()
    {
        directionController = GetComponent<CharacterDirectionController>();
    }

    public void FacePlayer(PlayerController player) =>
        directionController.UseOppositeOfDirection(player.Direction);
}
