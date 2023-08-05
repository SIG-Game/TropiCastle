﻿using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickupAndPlacement : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private CharacterItemInWorldController playerItemInWorld;
    [SerializeField] private CursorController cursorController;
    [SerializeField] private PlayerActionDisablingUIManager playerActionDisablingUIManager;
    [SerializeField] private Sprite itemPickupArrow;
    [SerializeField] private Sprite itemPickupArrowInventoryFull;
    [SerializeField] private Color canPlaceCursorBackgroundColor;
    [SerializeField] private Color cannotPlaceCursorBackgroundColor;
    [SerializeField] private InputActionReference itemPickupAndPlacementActionReference;

    public bool WaitingForInputReleaseBeforePlacement { get; set; }
    public ItemWorld HoveredItemWorld { get; private set; }
    public bool CanPlaceItemAtCursorPosition { get; private set; }
    public bool CursorIsOverItemWorld { get; private set; }
    public bool PlacingItem { get; private set; }

    public DefaultItemPickupAndPlacementState DefaultState { get; private set; }
    public ItemPickupState PickupState { get; private set; }
    public ItemPlacementState PlacementState { get; private set; }

    private BaseItemPickupAndPlacementState currentState;
    private Vector2 cursorPoint;
    private InputAction itemPickupAndPlacementAction;

    private void Awake()
    {
        WaitingForInputReleaseBeforePlacement = false;

        itemPickupAndPlacementAction = itemPickupAndPlacementActionReference.action;
    }

    private void Start()
    {
        DefaultState = new DefaultItemPickupAndPlacementState(this);
        PickupState = new ItemPickupState(this, itemPickupAndPlacementAction);
        PlacementState = new ItemPlacementState(this, itemPickupAndPlacementAction);

        currentState = DefaultState;

        currentState.StateEnter();
    }

    private void Update()
    {
        if (itemPickupAndPlacementAction.WasReleasedThisFrame())
        {
            WaitingForInputReleaseBeforePlacement = false;
        }

        if (PauseController.Instance.GamePaused ||
            playerActionDisablingUIManager.ActionDisablingUIOpen || player.IsAttacking)
        {
            bool shouldCancelPlacement = currentState == PlacementState;
            if (shouldCancelPlacement)
            {
                SwitchState(DefaultState);
            }

            return;
        }

        cursorPoint = cursorController.GetWorldPosition();
        UpdateHoveredItemWorld();
        UpdateCanPlaceItemAtCursorPosition();
        UpdatePlacingItem();

        currentState.StateUpdate();
    }

    public void AttemptToPickUpHoveredItem()
    {
        ItemWithAmount hoveredItem = HoveredItemWorld.GetItem();

        playerInventory.TryAddItemAtIndexWithFallbackToFirstEmptyIndex(
            hoveredItem, player.GetSelectedItemIndex(), out int amountAdded);

        bool hoveredItemAdded = amountAdded == hoveredItem.amount;
        bool hoveredItemPartiallyAdded = !hoveredItemAdded && amountAdded != 0;
        if (hoveredItemAdded)
        {
            Destroy(HoveredItemWorld.gameObject);
        }
        else if (hoveredItemPartiallyAdded)
        {
            HoveredItemWorld.SetItemAmount(hoveredItem.amount - amountAdded);
        }
        else
        {
            InventoryFullUIController.Instance.ShowInventoryFullText();
        }
    }

    public void PlaceSelectedItemAtCursorPosition()
    {
        ItemWithAmount itemToPlace = player.GetSelectedItem();
        int itemToPlaceIndex = player.GetSelectedItemIndex();

        if (itemToPlace.itemData.name != "Empty")
        {
            _ =ItemWorldPrefabInstanceFactory.Instance.SpawnItemWorld(cursorPoint, itemToPlace);
            playerInventory.RemoveItemAtIndex(itemToPlaceIndex);
        }
    }

    public void ResetCursor()
    {
        cursorController.UseDefaultCursor();
    }

    public void ResetCursorAndPlayerItem()
    {
        ResetCursor();
        playerItemInWorld.HideCharacterItemInWorld();
    }

    public void UsePickupCursor()
    {
        if (HoveredItemWorld != null &&
            !playerInventory.CanAddItem(HoveredItemWorld.GetItem(), out int canAddAmount) &&
            canAddAmount == 0)
        {
            cursorController.Sprite = itemPickupArrowInventoryFull;
        }
        else
        {
            cursorController.Sprite = itemPickupArrow;
        }
    }

    public void UsePlacementCursorAndPlayerItem()
    {
        ItemWithAmount selectedItem = player.GetSelectedItem();

        Color cursorBackgroundColor = CanPlaceItemAtCursorPosition ?
            canPlaceCursorBackgroundColor : cannotPlaceCursorBackgroundColor;

        Vector2 selectedItemColliderSize =
            ItemWorldPrefabInstanceFactory.GetItemColliderSize(selectedItem.itemData);

        cursorController.UpdateUsingItem(selectedItem);
        cursorController.UpdateCursorBackground(cursorBackgroundColor,
            selectedItemColliderSize);

        playerItemInWorld.ShowCharacterItemInWorld(selectedItem);
    }

    private void UpdateHoveredItemWorld()
    {
        Collider2D cursorOverlapCollider = Physics2D.OverlapPoint(cursorPoint);

        bool cursorIsOverCollider = cursorOverlapCollider != null;
        CursorIsOverItemWorld = cursorIsOverCollider && cursorOverlapCollider.CompareTag("Item World");

        if (CursorIsOverItemWorld)
        {
            HoveredItemWorld = cursorOverlapCollider.GetComponent<ItemWorld>();
        }
        else
        {
            HoveredItemWorld = null;
        }
    }

    private void UpdateCanPlaceItemAtCursorPosition()
    {
        ItemScriptableObject selectedItemData = player.GetSelectedItem().itemData;

        Vector2 selectedItemColliderExtents =
            ItemWorldPrefabInstanceFactory.GetItemColliderExtents(selectedItemData);

        CanPlaceItemAtCursorPosition = SpawnColliderHelper.CanSpawnColliderAtPosition(
            cursorPoint, selectedItemColliderExtents);
    }

    private void UpdatePlacingItem()
    {
        ItemScriptableObject selectedItemData = player.GetSelectedItem().itemData;

        PlacingItem = itemPickupAndPlacementAction.IsPressed() &&
            !WaitingForInputReleaseBeforePlacement && selectedItemData.name != "Empty";
    }

    public void SwitchState(BaseItemPickupAndPlacementState newState)
    {
        currentState.StateExit();
        currentState = newState;
        currentState.StateEnter();
    }
}
