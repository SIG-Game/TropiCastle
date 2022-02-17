using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject spawned;
    public float spawnTimer;
    public float spawnDelay;
    private bool isSpawned = false;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("spawnObject", spawnTimer, spawnDelay);
    }

    public void spawnObject() {
        //if (!isSpawned) {
            Instantiate(spawned, transform.position, transform.rotation);
            //isSpawned = !isSpawned;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
