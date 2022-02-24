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
        Debug.Log("Item interaction");
    }
}
