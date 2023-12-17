using System;
using UnityEngine;

public class CharacterDirectionController : MonoBehaviour
{
    [SerializeField] private DirectionalSprites sprites;
    [SerializeField] private CharacterDirection defaultDirection;

    public CharacterDirection Direction
    {
        get => direction;
        set
        {
            direction = value;

            spriteRenderer.sprite = sprites.ForDirection(direction);
        }
    }

    private SpriteRenderer spriteRenderer;
    private CharacterDirection direction;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        UseDefaultDirection();
    }

    public void UseDefaultDirection()
    {
        Direction = defaultDirection;
    }

    public Vector2 GetDirectionVector() =>
        Direction switch
        {
            CharacterDirection.Up => Vector2.up,
            CharacterDirection.Down => Vector2.down,
            CharacterDirection.Left => Vector2.left,
            CharacterDirection.Right => Vector2.right,
            _ => throw new ArgumentOutOfRangeException(nameof(Direction))
        };
}
