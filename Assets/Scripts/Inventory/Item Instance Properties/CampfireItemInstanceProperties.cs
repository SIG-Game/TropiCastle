using System;

[Serializable]
public class CampfireItemInstanceProperties : ContainerItemInstanceProperties
{
    public float CookTimeProgress;

    public override int InventorySize => 2;
}
