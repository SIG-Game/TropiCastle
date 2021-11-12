using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldSpawner : MonoBehaviour
{
    public Item item;

    // TODO: Maybe change back to Awake and fix execution order (ItemWorld before ItemWorldSpawner)
    private void Start()
    {
        ItemWorld.SpawnItemWorld(transform.position, item);
        Destroy(gameObject);
    }
}
