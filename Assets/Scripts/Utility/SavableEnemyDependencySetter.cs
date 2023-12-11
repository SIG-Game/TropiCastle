using UnityEngine;

public class SavableEnemyDependencySetter : MonoBehaviour,
    ISavablePrefabDependencySetter
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ItemWorldPrefabInstanceFactory itemWorldPrefabInstanceFactory;

    public void SetPrefabDependencies(SavablePrefab savablePrefab)
    {
        SavablePrefabEnemy savablePrefabEnemy = (SavablePrefabEnemy)savablePrefab;

        savablePrefabEnemy.GetEnemyController().SetUpEnemy(playerTransform,
            playerInventory, itemWorldPrefabInstanceFactory);
    }
}
