using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public static ItemWorld SpawnItemWorld(Vector3 position, Item.ItemType itemType, int amount)
    {
        Transform transform = Instantiate(ItemAssets.Instance.itemWorld, position, Quaternion.identity);

        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.itemType = itemType;
        itemWorld.amount = amount;
        itemWorld.spriteRenderer.sprite = Item.GetSprite(itemType);

        return itemWorld;
    }

    public static void DropItem(Vector3 dropPosition, Item.ItemType itemType, int amount)
    {
        if (itemType == Item.ItemType.Empty)
            return;

        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        randomOffset.Normalize();

        ItemWorld itemWorld = SpawnItemWorld(dropPosition + randomOffset, itemType, amount);

        itemWorld.GetComponent<Rigidbody2D>().AddForce(randomOffset * 0.5f, ForceMode2D.Impulse);
    }

    public Item.ItemType itemType;
    public int amount;
    private SpriteRenderer spriteRenderer;
    public bool spawnedFromSpawner;
    public ItemSpawner spawner;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Item.GetSprite(itemType);
    }
}
