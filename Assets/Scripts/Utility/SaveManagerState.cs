using System;
using System.Collections.Generic;

[Serializable]
public class SaveManagerState
{
    public string SaveGuid;
    public Dictionary<string, object> Properties;
}
