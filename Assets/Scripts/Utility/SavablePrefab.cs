using System.Collections.Generic;
using UnityEngine;

public abstract class SavablePrefab : MonoBehaviour
{
    public abstract string PrefabGameObjectName { get; }

    public SavablePrefabState GetSavablePrefabState()
    {
        var savablePrefabState = new SavablePrefabState
        {
            PrefabGameObjectName = PrefabGameObjectName,
            Properties = GetProperties()
        };

        return savablePrefabState;
    }

    public abstract Dictionary<string, object> GetProperties();

    public abstract void SetUpFromProperties(Dictionary<string, object> properties);
}
