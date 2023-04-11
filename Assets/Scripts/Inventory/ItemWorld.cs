using UnityEngine;

public class ItemWorld : Interactable
{
    [SerializeField] public ItemWithAmount item;

    public IItemInteraction ItemInteraction { private get; set; }

    // These operations must be in the Start method because the Awake
    // method runs before ItemWorldPrefabInstanceFactory sets item
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = item.itemData.sprite;

        if (item.itemData.hasCustomColliderSize) {
            GetComponent<BoxCollider2D>().size = item.itemData.customColliderSize;
        }

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
