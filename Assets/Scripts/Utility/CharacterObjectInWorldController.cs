using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterObjectInWorldController : MonoBehaviour
{
    [SerializeField] protected CharacterDirectionController characterDirectionController;

    // Direction order for this variable is up, down, left, right
    [SerializeField] private List<Vector3> objectInWorldOffsets;

    private SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void Show(Sprite objectSprite)
    {
        Vector3 itemInWorldOffset =
            objectInWorldOffsets[(int)characterDirectionController.Direction];

        if (characterDirectionController.Direction == CharacterDirection.Up)
        {
            spriteRenderer.sortingOrder = -1;
        }
        else
        {
            spriteRenderer.sortingOrder = 1;
        }

        transform.localPosition = itemInWorldOffset;

        spriteRenderer.sprite = objectSprite;
    }

    public void Hide()
    {
        spriteRenderer.sprite = null;
    }
}
