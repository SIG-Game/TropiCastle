using UnityEngine;

public class ItemWorld : Interactable
{
    [SerializeField] public ItemWithAmount item;

    public IItemInteraction ItemInteraction { private get; set; }

    private void Start()
    {
        // Not in Awake because this needs to happen after ItemWorldPrefabInstanceFactory sets item
        GetComponent<SpriteRenderer>().sprite = item.itemData.sprite;
        name = $"{item.itemData.name} ItemWorld";
    }

    public override void Interact(PlayerController player)
    {
        if (ItemInteraction != null)
        {
            Debug.Log("Item interaction with item interaction type " + ItemInteraction.GetType().Name);
            ItemInteraction.Interact(player);
        }
    }
}
