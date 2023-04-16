using System.Collections.Generic;
using UnityEngine;

public class PlayerItemInWorldController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform playerTransform;

    // Direction order for this variable is up, down, left, right
    [SerializeField] private List<Vector3> itemInWorldOffsets;

    private SpriteRenderer itemSpriteRenderer;

    private void Awake()
    {
        itemSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ShowPlayerItemInWorld(Sprite playerItemSprite)
    {
        Vector3 itemInWorldOffset = itemInWorldOffsets[(int)playerController.LastDirection];

        if (playerController.LastDirection == CharacterDirection.Up)
            itemSpriteRenderer.sortingOrder = -1;
        else
            itemSpriteRenderer.sortingOrder = 1;

        transform.localPosition = itemInWorldOffset;
        itemSpriteRenderer.sprite = playerItemSprite;
    }

    public void HidePlayerItemInWorld()
    {
        itemSpriteRenderer.sprite = null;
    }
}
