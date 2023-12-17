using System;
using UnityEngine;

[Serializable]
public class DirectionalSprites
{
    [SerializeField] private Sprite back;
    [SerializeField] private Sprite front;
    [SerializeField] private Sprite left;
    [SerializeField] private Sprite right;

    public Sprite Back => back;
    public Sprite Front => front;
    public Sprite Left => left;
    public Sprite Right => right;

    public Sprite ForDirection(CharacterDirection direction) =>
        direction switch
        {
            CharacterDirection.Up => Back,
            CharacterDirection.Down => Front,
            CharacterDirection.Left => Left,
            CharacterDirection.Right => Right,
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
}
