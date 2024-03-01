using System;
using UnityEngine;
using static CharacterDirection;

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

            OnDirectionSet();
        }
    }

    public event Action OnDirectionSet = () => {};

    private SpriteRenderer spriteRenderer;
    private CharacterDirection direction;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        UseDefaultDirection();
    }

    public void UseDefaultDirection() => Direction = defaultDirection;

    public void UseOppositeOfDirection(CharacterDirection direction) =>
        Direction = direction switch
        {
            Up => Down,
            Down => Up,
            Left => Right,
            Right => Left,
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };

    public Vector2 GetDirectionVector() =>
        Direction switch
        {
            Up => Vector2.up,
            Down => Vector2.down,
            Left => Vector2.left,
            Right => Vector2.right,
            _ => throw new ArgumentOutOfRangeException(nameof(Direction))
        };
}
