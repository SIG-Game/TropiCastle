using System;
using UnityEngine;

public class SavablePrefabEnemy : SavablePrefab
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private HealthController healthController;
    [SerializeField] private Spawnable spawnable;

    public override SavablePrefabState GetSavablePrefabState()
    {
        var savableState = new SavableEnemyState
        {
            Position = transform.position,
            Health = healthController.CurrentHealth,
            SpawnerGuid = spawnable.GetSpawnerGuid()
        };

        return savableState;
    }

    public override void SetUpFromSavablePrefabState(SavablePrefabState savableState)
    {
        SavableEnemyState enemyState = (SavableEnemyState)savableState;

        transform.position = enemyState.Position;

        healthController.CurrentHealth = enemyState.Health;

        spawnable
            .SetSpawnerUsingGuid<EnemySpawner>(enemyState.SpawnerGuid);
    }

    public override Type GetDependencySetterType() =>
        typeof(SavableEnemyDependencySetter);

    public EnemyController GetEnemyController() => enemyController;

    [Serializable]
    public class SavableEnemyState : SavablePrefabState
    {
        public Vector2 Position;
        public int Health;
        public string SpawnerGuid;

        public override string GetPrefabGameObjectName() => "Crab";
    }
}
