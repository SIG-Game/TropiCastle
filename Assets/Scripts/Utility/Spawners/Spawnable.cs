using UnityEngine;

public class Spawnable : MonoBehaviour
{
    private PrefabSpawner spawner;

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.SpawnedPrefabDestroyed();
        }
    }

    public void SetSpawner(PrefabSpawner spawner)
    {
        this.spawner = spawner;
    }

    public string GetSpawnerGameObjectName() =>
        spawner != null ? spawner.gameObject.name : string.Empty;
}
