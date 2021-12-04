using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnEnemy : MonoBehaviour
{
    public GameObject enemy;
    float randX;
    float randY;
    Vector2 whereToSpawn;
    public float spawnRate = 2f;
    float nextSpawn = 0.0f;
    float numCrabs = 0;
    public float maxCrabs = 4f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextSpawn && maxCrabs > numCrabs)
        {
            nextSpawn = Time.time + spawnRate;
            randX = Random.Range(-8f, 8f);
            randY = Random.Range(-3f, 4f);
            whereToSpawn = new Vector2(randX, randY);
            Instantiate(enemy, whereToSpawn, Quaternion.identity);
            numCrabs++;
        }
    }
}
