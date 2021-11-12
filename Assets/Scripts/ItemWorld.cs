using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
    {
        Transform transform = Instantiate(ItemAssets.Instance.itemWorld, position, Quaternion.identity);

        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);

        return itemWorld;
    }

    public static ItemWorld DropItem(Vector3 dropPosition, Item item)
    {
        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        randomOffset.Normalize();

        ItemWorld itemWorld = SpawnItemWorld(dropPosition + randomOffset, item);

        itemWorld.GetComponent<Rigidbody2D>().AddForce(randomOffset * 0.5f, ForceMode2D.Impulse);

        return itemWorld;
    }

    private Item item;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public Item GetItem()
    {
        return item;
    }

    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = item.GetSprite();
    }
}
