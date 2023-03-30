using System.Collections.Generic;
using UnityEngine;

public class PlayerFishItemInWorldController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform playerTransform;

    // Direction order for this variable is up, down, left, right
    [SerializeField] private List<Vector3> fishItemInWorldOffsets;

    private SpriteRenderer fishItemSpriteRenderer;

    private void Awake()
    {
        fishItemSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ShowPlayerFishItemInWorld(Sprite playerFishItemSprite)
    {
        Vector3 fishItemInWorldOffset = fishItemInWorldOffsets[(int)playerController.LastDirection];

        if (playerController.LastDirection == CharacterDirection.Up)
            fishItemSpriteRenderer.sortingOrder = -1;
        else
            fishItemSpriteRenderer.sortingOrder = 1;

        transform.localPosition = fishItemInWorldOffset;
        fishItemSpriteRenderer.sprite = playerFishItemSprite;
    }

    public void HidePlayerFishItemInWorld()
    {
        fishItemSpriteRenderer.sprite = null;
    }
}
