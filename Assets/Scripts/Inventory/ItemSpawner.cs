using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemWorldPrefab;
    public GameObject player;
    public float spawnDelay;
    public bool isSpawned = false;
    private float minX = -6f;
    private float maxX = 6f;
    private float minY = -2f;
    private float maxY = 4f;

    void Start()
    {
        InvokeRepeating("spawnObject", 0, spawnDelay);
    }

    public void spawnObject() {
        if (!isSpawned) {
            Vector2 spawnLocation = new Vector2(Random.Range(minX,maxX), Random.Range(minY,maxY));
            GameObject itemSpawned = Instantiate(itemWorldPrefab, spawnLocation, transform.rotation);
            isSpawned = true;

            ItemWorld itemWorldSpawned = itemSpawned.GetComponent<ItemWorld>();
            itemWorldSpawned.spawnedFromSpawner = true;
            itemWorldSpawned.spawner = this;

            Debug.Log($"Spawned item {itemWorldSpawned.item.info.name}");
        }
    }
}
