using System;

public abstract class NPCInteractable : Interactable
{
    protected CharacterDirectionController directionController;

    protected virtual void Awake()
    {
        directionController = GetComponent<CharacterDirectionController>();
    }

    public void FacePlayer(PlayerController player)
    {
        directionController.Direction = player.Direction switch
        {
            CharacterDirection.Up => CharacterDirection.Down,
            CharacterDirection.Down => CharacterDirection.Up,
            CharacterDirection.Left => CharacterDirection.Right,
            CharacterDirection.Right => CharacterDirection.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(player.Direction))
        };
    }
}
