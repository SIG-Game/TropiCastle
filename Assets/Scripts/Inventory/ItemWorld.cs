using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Inventory;

public class ItemWorld : MonoBehaviour, ISavablePrefabInstance
{
    [SerializeField] private ItemWithAmount item;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private bool logOnInteract;

    private static readonly Dictionary<string, Type> itemNameToInteractableType =
        new Dictionary<string, Type>
    {
        { "Campfire", typeof(CampfireItemInteractable) },
        { "Chest", typeof(ChestItemInteractable) }
    };

    private ItemInteractableDependencies itemInteractableDependencies;
    private Spawnable spawnable;

    private void Awake()
    {
        spawnable = GetComponent<Spawnable>();
    }

    public void SetUpItemWorld(ItemWithAmount item,
        ItemInteractableDependencies itemInteractableDependencies)
    {
        SetItemInteractableDependencies(itemInteractableDependencies);
        SetItem(item);
    }

    public void SetItemAmount(int amount)
    {
        item.amount = amount;

        amountText.text = item.GetAmountText();
    }

    public void SetItemInteractableDependencies(
        ItemInteractableDependencies itemInteractableDependencies)
    {
        this.itemInteractableDependencies = itemInteractableDependencies;
    }

    private void SetItem(ItemWithAmount item)
    {
        this.item = item;

        GetComponent<SpriteRenderer>().sprite = item.itemDefinition.sprite;

        if (item.itemDefinition.hasCustomColliderSize)
        {
            GetComponent<BoxCollider2D>().size = item.itemDefinition.customColliderSize;
        }

        if (itemNameToInteractableType.TryGetValue(item.itemDefinition.name,
            out Type itemInteractableType))
        {
            gameObject.AddComponent(itemInteractableType);

            GetComponent<ItemInteractable>()
                .SetUpUsingDependencies(itemInteractableDependencies);

            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        name = $"{item.itemDefinition.name} ItemWorld";

        amountText.text = item.GetAmountText();
    }

    public ItemWithAmount GetItem() => item;

    public SavablePrefabInstanceState GetSavablePrefabInstanceState()
    {
        var serializableItem = new SerializableInventoryItem(item);

        var savableState = new SavableItemWorldState
        {
            Item = serializableItem,
            Position = transform.position,
            SpawnerGuid = spawnable.GetSpawnerGuid()
        };

        return savableState;
    }

    public void SetPropertiesFromSavablePrefabInstanceState(
        SavablePrefabInstanceState savableState)
    {
        SavableItemWorldState itemWorldState = (SavableItemWorldState)savableState;

        transform.position = itemWorldState.Position;

        ItemScriptableObject itemScriptableObject =
            ItemScriptableObject.FromName(itemWorldState.Item.ItemName);

        ItemWithAmount item = new ItemWithAmount(itemScriptableObject,
            itemWorldState.Item.Amount,
            itemWorldState.Item.InstanceProperties);

        SetItem(item);

        GetComponent<Spawnable>()
            .SetSpawnerUsingGuid<ItemSpawner>(itemWorldState.SpawnerGuid);
    }

    [Serializable]
    public class SavableItemWorldState : SavablePrefabInstanceState
    {
        public SerializableInventoryItem Item;
        public Vector2 Position;
        public string SpawnerGuid;

        public override string GetSavablePrefabName() => "ItemWorld";
    }
}
