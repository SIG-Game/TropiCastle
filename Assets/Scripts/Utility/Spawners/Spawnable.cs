using System;
using System.Collections.Generic;
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

    public void SetSpawnerUsingId<TSpawner>(int spawnerId)
        where TSpawner : PrefabSpawner
    {
        if (spawnerId == -1)
        {
            return;
        }

        PrefabSpawner[] spawners = FindObjectsOfType<TSpawner>();

        PrefabSpawner spawner = spawners.FirstOrDefault(
            x => x.GetSpawnerId() == spawnerId);

        if (spawner != null)
        {
            SetSpawner(spawner);
        }
        else
        {
            Debug.LogWarning($"Spawner of type {typeof(TSpawner).Name} " +
                $"with spawner ID {spawnerId} not found");
        }
    }

    public int GetSpawnerId() =>
        spawner != null ? spawner.GetSpawnerId() : -1;
}
