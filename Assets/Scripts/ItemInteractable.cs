using UnityEngine;

public class ItemInteractable : Interactable
{
    PlayerController player;

    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();
    }

    public override void Interact()
    {
        Inventory playerInventory = player.GetInventory();

        if (!playerInventory.IsFull())
        {
            ItemWorld itemWorld = GetComponent<ItemWorld>();
            if (itemWorld.spawnedFromSpawner) {
                itemWorld.spawner.isSpawned = false;
            }
            player.GetInventory().AddItem(itemWorld.itemType, itemWorld.amount);
            Destroy(gameObject);
        }
    }
}
