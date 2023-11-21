using System;

[Serializable]
public class CampfireItemInstanceProperties : ContainerItemInstanceProperties
{
    public override int InventorySize => 2;

    public CampfireItemInstanceProperties()
    {
        AddProperty("CookTimeProgress", "0");
    }
}
