using System;

[Serializable]
public class ItemWithAmount
{
    public ItemScriptableObject itemData;
    public int amount;
    public object instanceProperties;

    public ItemWithAmount(ItemScriptableObject itemData, int amount,
        object instanceProperties = null)
    {
        this.itemData = itemData;
        this.amount = amount;
        this.instanceProperties = instanceProperties;
    }

    public void SetItemInstanceProperties()
    {
        if (itemData.name == "Chest")
        {
            instanceProperties = new ChestItemInstanceProperties();
        }
        else if (itemData.name == "Fishing Rod")
        {
            instanceProperties = new FishingRodItemInstanceProperties();
        }
    }
}
