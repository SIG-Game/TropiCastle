using UnityEngine;

public class SavableEnemyDependencySetter : MonoBehaviour,
    ISavablePrefabDependencySetter
{
    [SerializeField] private Inventory playerInventory;

    public void SetPrefabDependencies(SavablePrefab savablePrefab)
    {
        SavablePrefabEnemy savablePrefabEnemy = (SavablePrefabEnemy)savablePrefab;

        savablePrefabEnemy.GetEnemyController().SetPlayerInventory(playerInventory);
    }
}
