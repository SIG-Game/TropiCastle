using UnityEngine;

public class SavableItemWorldDependencySetter : MonoBehaviour,
    ISavablePrefabDependencySetter
{
    [SerializeField] private ItemInteractableDependencies itemInteractableDependencies;
    [SerializeField] private Transform itemWorldParent;

    public void SetPrefabDependencies(SavablePrefab savablePrefab)
    {
        SavablePrefabItemWorld savablePrefabItemWorld =
            (SavablePrefabItemWorld)savablePrefab;

        savablePrefabItemWorld.transform.parent = itemWorldParent;

        savablePrefabItemWorld.SetItemInteractableDependencies(
            itemInteractableDependencies);
    }
}
