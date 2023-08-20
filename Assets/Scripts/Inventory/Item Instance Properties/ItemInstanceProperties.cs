using System;

[Serializable]
public abstract class ItemInstanceProperties
{
    public virtual ItemInstanceProperties DeepCopy() =>
        (ItemInstanceProperties)MemberwiseClone();
}
