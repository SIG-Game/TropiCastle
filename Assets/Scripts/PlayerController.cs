using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float sprintSpeedMultiplier;
    public InventoryUI inventoryUI;
    public int maxHealth = 100;
    public int currentHealth;

    Rigidbody2D rb2d;

    private Inventory inventory;
    private int hotbarItemIndex = 0;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        inventory = new Inventory(UseItem);
        inventoryUI.SetInventory(inventory);
        inventoryUI.selectHotbarItem(hotbarItemIndex);

        // TODO: Remove (This line is for testing)
        ItemWorld.SpawnItemWorld(new Vector3(5f, 2.5f), Item.ItemType.Apple, 1);

        currentHealth = maxHealth;
    }
    
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 inputVector = (new Vector2(horizontalInput, verticalInput));
        inputVector.Normalize();

        Vector2 velocity = movementSpeed * inputVector;

        if (Input.GetButton("Sprint"))
        {
            velocity *= sprintSpeedMultiplier;
        }

        if (Input.GetButtonDown("Inventory"))
        {
            inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeInHierarchy);
        }

        rb2d.velocity = velocity;

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

        if (Input.GetKeyDown(KeyCode.E))
        {
            List<Item> itemList = inventory.GetItemList();

            UseItem(itemList[hotbarItemIndex]);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            List<Item> itemList = inventory.GetItemList();

            Item item = itemList[hotbarItemIndex];
            ItemWorld.DropItem(transform.position, item.itemType, item.amount);
            inventory.RemoveItem(item);
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
    }

    //increases health of a player when called
    //doesnt let health go above 100
    public void addHealth(int health)
    {
        if (currentHealth + health <= 100)
        {
            currentHealth += health;
        } else
        {
            currentHealth = 100;
        }
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
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // TODO: Use item tag instead
        ItemWorld itemWorld = collision.gameObject.GetComponent<ItemWorld>();
        if (itemWorld != null && !inventory.IsFull())
        {
            inventory.AddItem(itemWorld.itemType, itemWorld.amount);
            Destroy(itemWorld.gameObject);
        }
    }
}
