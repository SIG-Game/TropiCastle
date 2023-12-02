using System;
using UnityEngine;

[Serializable]
public class SerializableItem
{
    public string ItemName;
    public int Amount;

    [SerializeReference]
    public PropertyCollection InstanceProperties;

    public SerializableItem()
    {
    }

    public SerializableItem(SerializableItem serializableItem)
    {
        ItemName = serializableItem.ItemName;
        Amount = serializableItem.Amount;
        InstanceProperties = serializableItem.InstanceProperties?.DeepCopy();
    }

    public SerializableItem(ItemStack item)
    {
        ItemName = item.itemDefinition.name;
        Amount = item.amount;

        // Prevent serialization of empty instance properties
        if (item.instanceProperties != null &&
            item.instanceProperties.GetType() == typeof(PropertyCollection) &&
            item.instanceProperties.PropertyList.Count == 0)
        {
            InstanceProperties = null;
        }
        else
        {
            InstanceProperties = item.instanceProperties;
        }
    }
}
