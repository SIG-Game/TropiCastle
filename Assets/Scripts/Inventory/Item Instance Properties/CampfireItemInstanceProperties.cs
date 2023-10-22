using System.Collections.Generic;
using static Inventory;

public class CampfireItemInstanceProperties : ItemInstanceProperties
{
    public SerializableInventory SerializableInventory;

    public const int CampfireInventorySize = 2;

    public CampfireItemInstanceProperties()
    {
        List<SerializableInventoryItem> campfireItemList =
            new List<SerializableInventoryItem>(CampfireInventorySize);

        campfireItemList.Add(new SerializableInventoryItem
        {
            ItemName = "Empty",
            Amount = 0
        });

        SerializableInventory = new SerializableInventory
        {
            SerializableItemList = campfireItemList
        };
    }

    public void UpdateSerializableInventory(Inventory inventory)
    {
        SerializableInventory = inventory.GetAsSerializableInventory();
    }

    public override ItemInstanceProperties DeepCopy()
    {
        List<SerializableInventoryItem> campfireItemListDeepCopy =
            new List<SerializableInventoryItem>(CampfireInventorySize);

        for (int i = 0; i < CampfireInventorySize; ++i)
        {
            campfireItemListDeepCopy.Add(new SerializableInventoryItem(
                SerializableInventory.SerializableItemList[i]));
        }

        CampfireItemInstanceProperties deepCopy =
            (CampfireItemInstanceProperties)base.DeepCopy();

        deepCopy.SerializableInventory = new SerializableInventory
        {
            SerializableItemList = campfireItemListDeepCopy
        };

        return deepCopy;
    }
}
