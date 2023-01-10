﻿using UnityEngine;

public class ItemWorldPrefabInstanceFactory : MonoBehaviour
{
    [SerializeField] private GameObject itemWorldPrefab;

    public static ItemWorldPrefabInstanceFactory Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public ItemWorld SpawnItemWorld(Vector3 position, ItemWithAmount itemToSpawn)
    {
        GameObject spawnedGameObject = Instantiate(itemWorldPrefab, position, Quaternion.identity);
        ItemWorld spawnedItemWorld = spawnedGameObject.GetComponent<ItemWorld>();

        spawnedItemWorld.item = itemToSpawn;

        return spawnedItemWorld;
    }

    public void DropItem(Vector3 dropPosition, ItemWithAmount itemToDrop)
    {
        if (itemToDrop.itemData.name == "Empty")
            return;

        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        randomOffset.Normalize();
        randomOffset *= 0.7f;

        _ = SpawnItemWorld(dropPosition + randomOffset, itemToDrop);

        // Dropped items currently don't move
        //itemWorld.GetComponent<Rigidbody2D>().AddForce(randomOffset * 0.5f, ForceMode2D.Impulse);
    }
}