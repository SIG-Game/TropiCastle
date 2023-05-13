using System;

[Serializable]
public class ItemWithAmount
{
    public ItemScriptableObject itemData;
    public int amount;
    public object instanceProperties;

    public void SetItemInstanceProperties()
    {
        if (itemData.name == "Fishing Rod")
        {
            instanceProperties = new FishingRodItemInstanceProperties();
        }
    }
}
