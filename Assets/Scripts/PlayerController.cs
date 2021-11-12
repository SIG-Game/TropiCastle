using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float sprintSpeedMultiplier;
    public InventoryUI inventoryUI;

    Rigidbody2D rb2d;

    private Inventory inventory;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        inventory = new Inventory();
        inventoryUI.SetInventory(inventory);

        // TODO: Remove (This line is for testing)
        ItemWorld.SpawnItemWorld(new Vector3(5f, 2.5f), new Item { itemType = Item.ItemType.Test, amount = 1 });
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
