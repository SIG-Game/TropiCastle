using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject spawned;
    public GameObject player;
    public float spawnDelay;
    public bool isSpawned = false;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("spawnObject", 0, spawnDelay);
    }

    public void spawnObject() {
        if (!isSpawned) {
            GameObject itemSpawned = Instantiate(spawned, transform.position, transform.rotation);
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
