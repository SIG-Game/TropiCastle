using System.Collections.Generic;
using UnityEngine;

public class CharacterItemInWorldController : MonoBehaviour
{
    [SerializeField] private CharacterDirectionController characterDirectionController;
    [SerializeField] private Transform characterTransform;

    // Direction order for this variable is up, down, left, right
    [SerializeField] private List<Vector3> itemInWorldOffsets;

    private SpriteRenderer itemSpriteRenderer;

    private void Awake()
    {
        itemSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ShowCharacterItemInWorld(ItemWithAmount item)
    {
        Vector3 itemInWorldOffset =
            itemInWorldOffsets[(int)characterDirectionController.Direction];

        if (characterDirectionController.Direction == CharacterDirection.Up)
        {
            itemSpriteRenderer.sortingOrder = -1;
        }
        else
        {
            itemSpriteRenderer.sortingOrder = 1;
        }

        transform.localPosition = itemInWorldOffset;
        itemSpriteRenderer.sprite = item.itemDefinition.sprite;
    }

    public void HideCharacterItemInWorld()
    {
        itemSpriteRenderer.sprite = null;
    }
}
