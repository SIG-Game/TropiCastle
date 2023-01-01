﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum Direction { UP, DOWN, LEFT, RIGHT };

    public ItemScriptableObject emptyItemInfo;
    public InventoryUI inventoryUI;
    public Crafting crafting;
    public TextMeshProUGUI healthText;
    public GameObject pauseMenu;
    public GameObject gameOverUI;
    public int maxHealth = 100;
    public int currentHealth;
    public bool isAttacking = false;
    public bool dialogueBoxOpen = false;
    public bool canUseDialogueInputs = true;
    public bool gamePaused = false;

    public FishingMinigame fishingGame;

    public ItemPlacementTrigger itemPlacementTrigger;

    // TODO: This could be moved to a singleton
    public GameObject itemWorldPrefab;

    private bool canPause = true;
    private Animator animator;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    private Direction lastDirection = Direction.DOWN;
    private LayerMask interactableMask;
    private SpriteRenderer weaponSpriteRenderer;

    private Inventory inventory;
    private int hotbarItemIndex = 0;
    private bool inventoryOpen = false;
    private Vector2 velocity;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        weaponSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        inventory = new Inventory(UseItem, emptyItemInfo);
        inventoryUI.SetInventory(inventory);
        inventoryUI.selectHotbarItem(hotbarItemIndex);
        crafting.SetInventory(inventory);

        currentHealth = maxHealth;
        healthText.text = "Health: " + currentHealth;

        interactableMask = LayerMask.GetMask("Interactable");
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause") && !inventoryOpen && canPause) {
            TogglePauseMenu();
        }

        // Player must release next dialogue input to use that input
        // again for other actions after dialogue box closes
        if ((Input.GetButtonUp("Interact") || Input.GetMouseButtonUp(0)) &&
            !dialogueBoxOpen && !canUseDialogueInputs)
            canUseDialogueInputs = true;

        if (!gamePaused && !inventoryOpen)
        {
            if (Input.GetMouseButtonDown(0) && canUseDialogueInputs)
            {
                List<ItemWithAmount> itemList = inventory.GetItemList();

                UseItem(itemList[hotbarItemIndex]);

                // Prevent doing other actions on the frame an attack starts
                if (isAttacking)
                {
                    velocity = Vector2.zero;
                    return;
                }
            }

            if (Input.GetMouseButtonDown(1) && itemPlacementTrigger.canPlace)
            {
                List<ItemWithAmount> itemList = inventory.GetItemList();

                ItemWithAmount item = itemList[hotbarItemIndex];

                if (item.itemData.name != "Empty")
                {
                    Vector3 itemPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    // Lock item position to grid
                    //itemPosition.x = (Mathf.Round(2f * itemPosition.x - 0.5f) / 2f) + 0.25f;
                    //itemPosition.y = (Mathf.Round(2f * itemPosition.y - 0.5f) / 2f) + 0.25f;
                    itemPosition.z = 0f;

                    ItemWorld.SpawnItemWorld(itemWorldPrefab, itemPosition, item);
                    inventory.RemoveItem(item);
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

            if (Input.GetButtonDown("Interact") && canUseDialogueInputs && !gamePaused)
            {
                Vector2 interactionDirection = GetInteractionDirection();

                Vector3 raycastOrigin = transform.position;
                raycastOrigin.x += boxCollider.offset.x;
                raycastOrigin.y += boxCollider.offset.y;

                // TODO: Shorten length and change length based on direction
                RaycastHit2D hit = Physics2D.BoxCast(raycastOrigin,
                    boxCollider.size, 0f, interactionDirection, 0.15f, interactableMask);

                // Debug.DrawRay(raycastOrigin, interactionDirection * 0.25f, Color.red);

                if (hit.collider != null)
                {
                    hit.transform.gameObject.GetComponent<Interactable>().Interact(this);
                }
            }

            // TODO: Use mouse for picking up items
            if (Input.GetButtonDown("Pick Up") && !gamePaused)
            {
                Vector2 interactionDirection = GetInteractionDirection();

                Vector3 raycastOrigin = transform.position;
                raycastOrigin.x += boxCollider.offset.x;
                raycastOrigin.y += boxCollider.offset.y;

                RaycastHit2D hit = Physics2D.BoxCast(raycastOrigin,
                    boxCollider.size, 0f, interactionDirection, 0.15f, interactableMask);

                if (hit.collider != null && hit.collider.GetComponent<ItemWorld>() != null &&
                    !inventory.IsFull())
                {
                    ItemWorld itemWorld = hit.collider.GetComponent<ItemWorld>();
                    if (itemWorld.spawnedFromSpawner)
                        itemWorld.spawner.isSpawned = false;
                    inventory.AddItem(itemWorld.item);
                    Destroy(hit.collider.gameObject);
                }
            }
        }

        if (Input.GetButtonDown("Inventory") && !gamePaused)
        {
            inventoryOpen = !inventoryOpen;
            inventoryUI.gameObject.SetActive(inventoryOpen);
            rb2d.velocity = Vector2.zero;
            Time.timeScale = inventoryOpen ? 0f : 1f;

            if (!inventoryOpen)
            {
                inventoryUI.ResetHeldItem();
            }
        }

        // Debug input for fishing
        if (Input.GetKeyDown(KeyCode.G))
        {
            // TODO: Resources.Load calls should maybe use Addressables instead
            ItemScriptableObject fishingRodScriptableObject = Resources.Load<ItemScriptableObject>("Items/FishingRod");

            inventory.AddItem(fishingRodScriptableObject, 1);
            fishingGame.startFishing();
        }
    }

    private void FixedUpdate()
    {
        rb2d.MovePosition(transform.position + (Vector3)velocity);
    }

    public bool CanMove()
    {
        return !dialogueBoxOpen && !isAttacking && !gamePaused && !inventoryOpen;
    }

    //increases health of a player when called
    //doesnt let health go above 100
    public void addHealth(int health)
    {
        currentHealth += health;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthText.text = "Health: " + currentHealth;
    }

    //reduces health of player when this is called
    //doesnt let health go below 0
    public void takeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        healthText.text = "Health: " + currentHealth;

        if (currentHealth == 0)
        {
            Time.timeScale = 0f;
            canPause = false;
            gamePaused = true;
            gameOverUI.SetActive(true);
        }
    }

    public void Attack() {
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

    public Direction getLastDir() {
        return lastDirection;
    }

    public void SetLastDirection(Direction newLastDirection)
    {
        lastDirection = newLastDirection;
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        velocity = newVelocity;
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
        switch (item.itemData.name)
        {
            case "Empty":
                Debug.Log("Empty item used");
                break;
            case "Test":
                Debug.Log("Test item used");
                break;
            case "Apple":
                addHealth(10);
                inventory.RemoveItem(item);
                break;
            case "Stick":
                weaponSpriteRenderer.sprite = WeaponAssets.Instance.stickSprite;
                transform.GetChild(0).GetComponent<AttackScript>().damage = 40;
                Attack();
                break;
            case "Spear":
                weaponSpriteRenderer.sprite = WeaponAssets.Instance.spearSprite;
                transform.GetChild(0).GetComponent<AttackScript>().damage = 60;
                Attack();
                break;
            case "Rock":
                Debug.Log("Rock item used");
                break;
            case "Vine":
                Debug.Log("Vine item used");
                break;
            case "RawCrabMeat":
                addHealth(10);
                inventory.RemoveItem(item);
                break;
            case "CookedCrabMeat":
                addHealth(20);
                inventory.RemoveItem(item);
                break;
            case "FishingRod":
                Debug.Log("Fishing Rod used");
                fishingGame.startFishing();
                return;
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

    public void TogglePauseMenu()
    {
        gamePaused = !gamePaused;
        pauseMenu.SetActive(gamePaused);
        rb2d.velocity = Vector2.zero;
        Time.timeScale = gamePaused ? 0f : 1f;
    }

    public ItemWithAmount GetHotbarItem()
    {
        return inventory.GetItemList()[hotbarItemIndex];
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            takeDamage(10);
        }
        if(col.gameObject.CompareTag("Bullet"))
        {
            takeDamage(10);
        }
    }
}
