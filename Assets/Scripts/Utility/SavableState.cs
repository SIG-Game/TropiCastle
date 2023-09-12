using System;

[Serializable]
public class SavableState
{
    public string SaveGuid;

    public virtual Type GetSavableClassType() => null;
}
