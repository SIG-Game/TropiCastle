using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum Direction { UP, DOWN, LEFT, RIGHT };

    public float movementSpeed;
    public float sprintSpeedMultiplier;
    public InventoryUI inventoryUI;
    public Crafting crafting;
    public TextMeshProUGUI healthText;
    public GameObject pauseMenu;
    public int maxHealth = 100;
    public int currentHealth;
    public bool isAttacking = false;
    public bool dialogueBoxOpen = false;
    public bool canInteract = true;
    public bool gamePaused = false;

    public GameObject canvas;
    public GameObject Hook;
    public GameObject fish;

    public ItemPlacementTrigger itemPlacementTrigger;

    public Sprite front, back, left, right;
    
    private SpriteRenderer spriteRenderer;
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
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        weaponSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        inventory = new Inventory(UseItem);
        inventoryUI.SetInventory(inventory);
        inventoryUI.selectHotbarItem(hotbarItemIndex);
        crafting.SetInventory(inventory);

        // TODO: Remove (This line is for testing)
        ItemWorld.SpawnItemWorld(new Vector3(5f, 2.5f), Item.ItemType.Apple, 1);

        currentHealth = maxHealth;
        healthText.text = "Health: " + currentHealth;

        interactableMask = LayerMask.GetMask("Interactable");
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Pause") && !inventoryOpen) {
            TogglePauseMenu();
        }

        if (dialogueBoxOpen || isAttacking)
        {
            velocity = Vector2.zero;
            return;
        }

        // Player must release interact key to interact again after dialogue box closes
        // Needed because interact key is also used to advance dialogue
        if (Input.GetButtonUp("Interact") && !dialogueBoxOpen && !canInteract && !gamePaused)
        {
            canInteract = true;
        }

        if (!gamePaused && !inventoryOpen)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector2 inputVector = new Vector2(horizontalInput, verticalInput);
            
            if (inputVector.sqrMagnitude > 1f)
            {
                inputVector.Normalize();
            }

            velocity = movementSpeed * inputVector;

            if (Input.GetButton("Sprint"))
            {
                velocity *= sprintSpeedMultiplier;
            }

            if (Input.GetMouseButtonDown(0))
            {
                List<Item> itemList = inventory.GetItemList();

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
                List<Item> itemList = inventory.GetItemList();

                Item item = itemList[hotbarItemIndex];

                if (item.itemType != Item.ItemType.Empty)
                {
                    Vector3 itemPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    // Lock item position to grid
                    //itemPosition.x = (Mathf.Round(2f * itemPosition.x - 0.5f) / 2f) + 0.25f;
                    //itemPosition.y = (Mathf.Round(2f * itemPosition.y - 0.5f) / 2f) + 0.25f;
                    itemPosition.z = 0f;
                    ItemWorld.SpawnItemWorld(itemPosition, item.itemType, item.amount);
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

            if (horizontalInput != 0 || verticalInput != 0) {
                if (horizontalInput < 0)
                {
                    lastDirection = Direction.LEFT;
                    spriteRenderer.sprite = left;
                }
                else if (horizontalInput > 0)
                {
                    lastDirection = Direction.RIGHT;
                    spriteRenderer.sprite = right;
                }
                else if (verticalInput < 0)
                {
                    lastDirection = Direction.DOWN;
                    spriteRenderer.sprite = front;
                } 
                else
                {
                    lastDirection = Direction.UP;
                    spriteRenderer.sprite = back;
                }
            }

            if (Input.GetButtonDown("Interact") && canInteract && !gamePaused)
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
                    hit.transform.gameObject.GetComponent<Interactable>().Interact();
                }
            }

            if (Input.GetButtonDown("Pick Up") && !gamePaused)
            {
                // TODO: Use method for casting a box from player
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
                    inventory.AddItem(itemWorld.itemType, itemWorld.amount);
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

        // Add items for testing
        if (Input.GetKeyDown(KeyCode.Z))
        {
            inventory.AddItem(Item.ItemType.Test, 1);
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            inventory.AddItem(Item.ItemType.Apple, 1);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            inventory.AddItem(Item.ItemType.Stick, 1);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            inventory.AddItem(Item.ItemType.Spear, 1);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            inventory.AddItem(Item.ItemType.Rock, 1);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            inventory.AddItem(Item.ItemType.Vine, 1);
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            inventory.AddItem(Item.ItemType.RawCrabMeat, 1);
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            inventory.AddItem(Item.ItemType.Campfire, 1);
        }
        
        if (Input.GetKeyDown(KeyCode.G)) 
        {
            if (!canvas.activeSelf){
                fish.GetComponent<RectTransform>().anchoredPosition = new Vector3(Random.Range(-190,190),0,0);
                canvas.SetActive(true);
            }
            
        }
    }

    private void FixedUpdate()
    {
        rb2d.MovePosition(transform.position + (Vector3)velocity);
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

    private void UseItem(Item item)
    {
        switch (item.itemType)
        {
            case Item.ItemType.Empty:
                Debug.Log("Empty item used");
                break;
            case Item.ItemType.Test:
                Debug.Log("Test item used");
                break;
            case Item.ItemType.Apple:
                addHealth(10);
                inventory.RemoveItem(item);
                break;
            case Item.ItemType.Stick:
                weaponSpriteRenderer.sprite = WeaponAssets.Instance.stickSprite;
                transform.GetChild(0).GetComponent<AttackScript>().damage = 40;
                Attack();
                break;
            case Item.ItemType.Spear:
                weaponSpriteRenderer.sprite = WeaponAssets.Instance.spearSprite;
                transform.GetChild(0).GetComponent<AttackScript>().damage = 60;
                Attack();
                return;
            case Item.ItemType.Rock:
                Debug.Log("Rock item used");
                return;
            case Item.ItemType.Vine:
                Debug.Log("Vine item used");
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

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            takeDamage(10);
        }
        if (col.gameObject.CompareTag("Water")) 
        {
            movementSpeed *= 0.5f;
        }
    }

    void OnTriggerExit2D(Collider2D col) 
    {
        if (col.gameObject.CompareTag("Water")) {
            movementSpeed *= 2f;
        }
    }
}
