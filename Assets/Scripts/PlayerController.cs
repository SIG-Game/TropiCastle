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

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        inventory = new Inventory();
        inventoryUI.SetInventory(inventory);

        // TODO: Remove (This line is for testing)
        ItemWorld.SpawnItemWorld(new Vector3(5f, 2.5f), new Item { itemType = Item.ItemType.Test, amount = 1 });

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
        
        /* test takeDamage
        if(Input.GetKeyDown(KeyCode.Space))
        {
            takeDamage(20);
        }
        */
    }

    //reduces health of player when this is called
    public void takeDamage(int damage)
    {
        if (currentHealth - damage >= 0) //doesnt let health go below 0 
        {
            currentHealth -= damage;
        } else
        {
            currentHealth = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: Use item tag instead
        ItemWorld itemWorld = collision.gameObject.GetComponent<ItemWorld>();
        if (itemWorld != null)
        {
            inventory.AddItem(itemWorld.GetItem());
            inventoryUI.RefreshInventoryItems();
            Destroy(itemWorld.gameObject);
        }
    }
}
