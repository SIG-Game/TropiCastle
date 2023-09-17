using UnityEngine;

public class SavableEnemyDependencySetter : MonoBehaviour,
    ISavablePrefabInstanceDependencySetter
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Inventory playerInventory;

    public void SetPrefabInstanceDependencies(ISavablePrefabInstance savablePrefabInstance)
    {
        EnemyController enemy = (EnemyController)savablePrefabInstance;

        enemy.SetUpEnemy(playerTransform, playerInventory);
    }
}
