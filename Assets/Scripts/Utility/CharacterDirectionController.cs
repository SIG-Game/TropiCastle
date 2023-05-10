using System;
using UnityEngine;

public class CharacterDirectionController : MonoBehaviour
{
    [SerializeField] private Sprite front, back, left, right;
    [SerializeField] private CharacterDirection defaultDirection;

    public CharacterDirection Direction
    {
        get => direction;
        set
        {
            direction = value;

            spriteRenderer.sprite = direction switch
            {
                CharacterDirection.Up => back,
                CharacterDirection.Down => front,
                CharacterDirection.Left => left,
                CharacterDirection.Right => right,
                _ => throw new ArgumentOutOfRangeException(nameof(direction))
            };
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
}