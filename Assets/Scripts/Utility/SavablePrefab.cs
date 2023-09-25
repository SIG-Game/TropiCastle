using System;
using UnityEngine;

public abstract class SavablePrefab : MonoBehaviour
{
    public abstract SavablePrefabState GetSavablePrefabState();

    public abstract void SetUpFromSavablePrefabState(
        SavablePrefabState savableState);

    public abstract Type GetDependencySetterType();
}
