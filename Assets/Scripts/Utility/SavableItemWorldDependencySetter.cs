using UnityEngine;

public class SavableItemWorldDependencySetter : MonoBehaviour,
    ISavablePrefabInstanceDependencySetter
{
    [SerializeField] private ItemInteractableDependencies itemInteractableDependencies;
    [SerializeField] private Transform itemWorldParent;

    public void SetPrefabInstanceDependencies(ISavablePrefabInstance savablePrefabInstance)
    {
        ItemWorld itemWorld = (ItemWorld)savablePrefabInstance;

        itemWorld.transform.parent = itemWorldParent;

        itemWorld.SetItemInteractableDependencies(itemInteractableDependencies);
    }
}
