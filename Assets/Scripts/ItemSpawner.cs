using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject spawned;
    public GameObject player;
    public float spawnDelay;
    public bool isSpawned = false;
    private float minX = -6f;
    private float maxX = 6f;
    private float minY = -2f;
    private float maxY = 4f;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("spawnObject", 0, spawnDelay);
    }

    public void spawnObject() {
        if (!isSpawned) {
            Vector2 spawnLocation = new Vector2(Random.Range(minX,maxX), Random.Range(minY,maxY));
            GameObject itemSpawned = Instantiate(spawned, spawnLocation, transform.rotation);
            isSpawned = true;
            ItemWorld iwtest = itemSpawned.GetComponent<ItemWorld>();
            iwtest.spawnedFromSpawner = true;
            iwtest.spawner = this;
            Debug.Log("spawned");
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
