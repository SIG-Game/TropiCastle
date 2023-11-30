using System;
using System.Collections.Generic;

[Serializable]
public class SavablePrefabState
{
    public string PrefabGameObjectName;
    public Dictionary<string, object> Properties;
}
