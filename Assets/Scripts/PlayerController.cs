using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum Direction { UP, DOWN, LEFT, RIGHT };

    public float movementSpeed;
    public float sprintSpeedMultiplier;
    public InventoryUI inventoryUI;
    public TextMeshProUGUI healthText;
    public int maxHealth = 100;
    public int currentHealth;
    public bool isAttacking = false;

    public Sprite front, back, left, right;
    
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    private Direction lastDirection = Direction.DOWN;
    private LayerMask interactableMask;

    private Inventory inventory;
    private int hotbarItemIndex = 0;
    private bool gamePaused = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        inventory = new Inventory(UseItem);
        inventoryUI.SetInventory(inventory);
        inventoryUI.selectHotbarItem(hotbarItemIndex);

        // TODO: Remove (This line is for testing)
        ItemWorld.SpawnItemWorld(new Vector3(5f, 2.5f), Item.ItemType.Apple, 1);

        currentHealth = maxHealth;
        healthText.text = "Health: " + currentHealth;

        interactableMask = LayerMask.GetMask("Interactable");
    }
    
    void Update()
    {
        if (!gamePaused)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector2 inputVector = new Vector2(horizontalInput, verticalInput);
            
            if (inputVector.sqrMagnitude > 1f)
            {
                inputVector.Normalize();
            }

            Vector2 velocity = movementSpeed * inputVector;

            if (Input.GetButton("Sprint"))
            {
                velocity *= sprintSpeedMultiplier;
            }

            rb2d.velocity = velocity;

            if (isAttacking)
            {
                rb2d.velocity = Vector2.zero;
            }

            if (Input.mouseScrollDelta.y < 0f)
            {
                ++hotbarItemIndex;

                if (hotbarItemIndex == 10)
                    hotbarItemIndex = 0;

                inventoryUI.selectHotbarItem(hotbarItemIndex);
            }
            else if (Input.mouseScrollDelta.y > 0f)
            {
                --hotbarItemIndex;

                if (hotbarItemIndex == -1)
                    hotbarItemIndex = 9;

                inventoryUI.selectHotbarItem(hotbarItemIndex);
            }

            ProcessNumberKeys();

            if (Input.GetMouseButtonDown(0))
            {
                List<Item> itemList = inventory.GetItemList();

                UseItem(itemList[hotbarItemIndex]);
            }

            if (Input.GetMouseButtonDown(1))
            {
                List<Item> itemList = inventory.GetItemList();

                Item item = itemList[hotbarItemIndex];
                ItemWorld.DropItem(transform.position, item.itemType, item.amount);
                inventory.RemoveItem(item);
            }

            if ((horizontalInput != 0 || verticalInput != 0) && !isAttacking) {
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

            if (Input.GetButtonDown("Interact"))
            {
                Vector2 interactionDirection = GetInteractionDirection();

                Vector3 raycastOrigin = transform.position;
                raycastOrigin.x += boxCollider.offset.x;
                raycastOrigin.y += boxCollider.offset.y;

                // TODO: Shorten length and change length based on direction
                RaycastHit2D hit = Physics2D.Raycast(raycastOrigin,
                    interactionDirection, 0.25f, interactableMask);

                // Debug.DrawRay(raycastOrigin, interactionDirection * 0.25f, Color.red);

                if (hit.collider != null)
                {
                    hit.transform.gameObject.GetComponent<Interactable>().Interact();
                }
            }
        }

        if (Input.GetButtonDown("Inventory"))
        {
            gamePaused = !gamePaused;
            inventoryUI.gameObject.SetActive(gamePaused);
            rb2d.velocity = Vector2.zero;
            Time.timeScale = gamePaused ? 0f : 1f;

            if (!gamePaused)
            {
                inventoryUI.ResetHeldItem();
            }
        }

        // Add item test
        if (Input.GetKeyDown(KeyCode.C))
        {
            inventory.AddItem(Item.ItemType.Test, 1);
        }

        /*
        //test takeDamage
        if(Input.GetKeyDown(KeyCode.Space))
        {
            takeDamage(20);
        }
        
        //test getHealth
        if(Input.GetKeyDown(KeyCode.M))
        {
            addHealth(20);
        }
        */
    }

    //reduces health of player when this is called
    //doesnt let health go below 0
    public void takeDamage(int damage)
    {
        if (currentHealth - damage >= 0)  
        {
            currentHealth -= damage;
        } else
        {
            currentHealth = 0;
        }

        healthText.text = "Health: " + currentHealth;
    }

    public void Attack() {
        if (!isAttacking) {
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
        }
    }

    public Direction getLastDir() {
        return lastDirection;
    }
    
    //increases health of a player when called
    //doesnt let health go above 100
    public void addHealth(int health)
    {
        if(currentHealth + health <= maxHealth)
        {
            currentHealth += health;
        } else
        {
            currentHealth = maxHealth;
        }

        healthText.text = "Health: " + currentHealth;
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("Enemy"))
        {
            takeDamage(10);
        }
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
                Debug.Log("Apple item used");
                addHealth(10);
                inventory.RemoveItem(item);
                break;
            case Item.ItemType.Stick:
                Debug.Log("Stick item used");
                Attack();
                break;
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        ItemWorld itemWorld = collision.gameObject.GetComponent<ItemWorld>();
        if (itemWorld != null && !inventory.IsFull())
        {
            inventory.AddItem(itemWorld.itemType, itemWorld.amount);
            Destroy(itemWorld.gameObject);
        }
    }
}
