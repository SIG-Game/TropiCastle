using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum Direction { UP, DOWN, LEFT, RIGHT };

    public InventoryUI inventoryUI;
    public GameObject gameOverUI;
    public bool isAttacking = false;

    public FishingMinigame fishingGame;

    public Direction lastDirection { get; set; }

    private Animator animator;
    private BoxCollider2D boxCollider;
    private HealthController healthController;
    private LayerMask interactableMask;
    private SpriteRenderer weaponSpriteRenderer;
    private WeaponController weaponController;

    private Inventory inventory;
    private int hotbarItemIndex = 0;

    void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        healthController = GetComponent<HealthController>();

        weaponSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        weaponController = transform.GetChild(0).GetComponent<WeaponController>();

        ItemScriptableObject emptyItemInfo = Resources.Load<ItemScriptableObject>("Items/Empty");
        inventory = new Inventory(UseItem, emptyItemInfo);
        inventoryUI.SetInventory(inventory);
        inventoryUI.selectHotbarItem(hotbarItemIndex);

        lastDirection = Direction.DOWN;

        interactableMask = LayerMask.GetMask("Interactable");

        healthController.OnHealthChanged += HealthController_OnHealthChanged;
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory") &&
            (!PauseController.Instance.GamePaused || inventoryUI.gameObject.activeInHierarchy))
        {
            PauseController.Instance.GamePaused = !PauseController.Instance.GamePaused;
            inventoryUI.gameObject.SetActive(PauseController.Instance.GamePaused);

            if (!PauseController.Instance.GamePaused)
            {
                inventoryUI.ResetHeldItem();
            }
        }

        if (PauseController.Instance.GamePaused)
        {
            return;
        }

        if (InputManager.Instance.GetLeftClickDownIfUnusedThisFrame())
        {
            List<ItemWithAmount> itemList = inventory.GetItemList();

            UseItem(itemList[hotbarItemIndex]);

            // Prevent doing other actions on the frame an attack starts
            if (isAttacking)
            {
                return;
            }
        }

        if (Input.mouseScrollDelta.y != 0f)
        {
            hotbarItemIndex -= (int)Mathf.Sign(Input.mouseScrollDelta.y);

            if (hotbarItemIndex == 10)
                hotbarItemIndex = 0;
            else if (hotbarItemIndex == -1)
                hotbarItemIndex = 9;

            inventoryUI.selectHotbarItem(hotbarItemIndex);
        }

        ProcessNumberKeys();

        if (InputManager.Instance.GetInteractButtonDownIfUnusedThisFrame())
        {
            RaycastHit2D hit = InteractionCast();

            if (hit.collider != null)
            {
                hit.transform.gameObject.GetComponent<Interactable>().Interact(this);
            }
        }

        // Debug input for fishing
        if (Input.GetKeyDown(KeyCode.G))
        {
            // TODO: Resources.Load calls should maybe use Addressables instead
            ItemScriptableObject fishingRodScriptableObject = Resources.Load<ItemScriptableObject>("Items/FishingRod");

            inventory.AddItem(fishingRodScriptableObject, 1);
            StartCoroutine(fishingGame.StartFishing());
        }
    }

    private RaycastHit2D InteractionCast()
    {
        Vector2 interactionDirection = GetInteractionDirection();

        Vector3 raycastOrigin = transform.position;
        raycastOrigin.x += boxCollider.offset.x;
        raycastOrigin.y += boxCollider.offset.y;

        // TODO: Shorten length and change length based on direction
        RaycastHit2D hit = Physics2D.BoxCast(raycastOrigin,
            boxCollider.size, 0f, interactionDirection, 0.15f, interactableMask);

        // Debug.DrawRay(raycastOrigin, interactionDirection * 0.25f, Color.red);

        return hit;
    }

    public bool CanMove()
    {
        return !isAttacking && !PauseController.Instance.GamePaused && !DialogueBox.Instance.DialogueBoxOpen();
    }

    public void PlayerDeath()
    {
        PauseController.Instance.GamePaused = true;
        gameOverUI.SetActive(true);
    }

    public void AttackWithWeapon(WeaponItemScriptableObject weaponItemData) {
        weaponSpriteRenderer.sprite = weaponItemData.weaponSprite;
        weaponController.damage = weaponItemData.damage;

        switch (lastDirection) {
            case Direction.UP:
                animator.Play("Swing Up");
                break;

            case Direction.DOWN:
                animator.Play("Swing Down");
                break;

            case Direction.LEFT:
                animator.Play("Swing Left");
                break;

            case Direction.RIGHT:
                animator.Play("Swing Right");
                break;
        }

        isAttacking = true;
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    Vector2 GetInteractionDirection()
    {
        switch (lastDirection)
        {
            case Direction.UP:
                return Vector2.up;

            case Direction.DOWN:
                return Vector2.down;

            case Direction.LEFT:
                return Vector2.left;

            case Direction.RIGHT:
                return Vector2.right;
        }

        // Should not be reached
        return Vector2.zero;
    }

    private void UseItem(ItemWithAmount item)
    {
        if (item.itemData is HealingItemScriptableObject healingItemData)
        {
            ConsumeHealingItem(item, healingItemData.healAmount);
        }
        else if (item.itemData is WeaponItemScriptableObject weaponItemData)
        {
            AttackWithWeapon(weaponItemData);
        }
        else if (item.itemData.name == "FishingRod")
        {
            StartCoroutine(fishingGame.StartFishing());
        }
        else
        {
            Debug.Log($"Used item named {item.itemData.name}, which has no usage defined.");
        }
    }

    private void ConsumeHealingItem(ItemWithAmount item, int amountToHeal)
    {
        if (!healthController.AtMaxHealth())
        {
            healthController.IncreaseHealth(amountToHeal);
            inventory.RemoveItem(item);
        }
    }

    private void ProcessNumberKeys()
    {
        int previousHotbarItemIndex = hotbarItemIndex;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            hotbarItemIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            hotbarItemIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            hotbarItemIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            hotbarItemIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            hotbarItemIndex = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            hotbarItemIndex = 5;
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            hotbarItemIndex = 6;
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            hotbarItemIndex = 7;
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            hotbarItemIndex = 8;
        else if (Input.GetKeyDown(KeyCode.Alpha0))
            hotbarItemIndex = 9;

        if (previousHotbarItemIndex != hotbarItemIndex)
            inventoryUI.selectHotbarItem(hotbarItemIndex);
    }

    private void HealthController_OnHealthChanged(int newHealth)
    {
        if (newHealth == 0)
        {
            PlayerDeath();
        }
    }

    public ItemWithAmount GetHotbarItem()
    {
        return inventory.GetItemList()[hotbarItemIndex];
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Bullet"))
        {
            healthController.DecreaseHealth(10);
        }
    }
}
