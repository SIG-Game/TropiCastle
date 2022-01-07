﻿using System.Collections.Generic;
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
    private bool gamePaused = false;

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
        if (!gamePaused)
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

            ProcessNumberKeys();

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
        // TODO: Use item tag instead
        ItemWorld itemWorld = collision.gameObject.GetComponent<ItemWorld>();
        if (itemWorld != null && !inventory.IsFull())
        {
            inventory.AddItem(itemWorld.itemType, itemWorld.amount);
            Destroy(itemWorld.gameObject);
        }
    }
}
