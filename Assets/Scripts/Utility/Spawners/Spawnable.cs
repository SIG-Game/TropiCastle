using System.Linq;
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

    public void SetSpawnerUsingGuid<TSpawner>(string spawnerGuid)
        where TSpawner : PrefabSpawner
    {
        if (string.IsNullOrEmpty(spawnerGuid))
        {
            return;
        }

        PrefabSpawner[] spawners = FindObjectsOfType<TSpawner>();

        PrefabSpawner spawner = spawners.FirstOrDefault(
            x => x.GetSpawnerGuid() == spawnerGuid);

        if (spawner != null)
        {
            SetSpawner(spawner);
        }
        else
        {
            Debug.LogWarning($"Spawner of type {typeof(TSpawner).Name} " +
                $"with spawner GUID {spawnerGuid} not found");
        }
    }

    public string GetSpawnerGuid() =>
        spawner != null ? spawner.GetSpawnerGuid() : string.Empty;
}
