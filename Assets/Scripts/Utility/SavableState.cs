using System;

[Serializable]
public abstract class SavableState
{
    public string SaveGuid;

    public abstract Type GetSavableClassType();
}
