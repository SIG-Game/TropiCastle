using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    private PrefabSpawner spawner;

    private static readonly Dictionary<Type, PrefabSpawner[]> spawnerCache;

    static Spawnable()
    {
        spawnerCache = new Dictionary<Type, PrefabSpawner[]>();
    }

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

        if (!spawnerCache.ContainsKey(typeof(TSpawner)))
        {
            spawnerCache[typeof(TSpawner)] = FindObjectsOfType<TSpawner>();
        }

        PrefabSpawner[] spawners = spawnerCache[typeof(TSpawner)];

        PrefabSpawner spawner = spawners.FirstOrDefault(
            x => x.GetSpawnerId() == spawnerId);

        if (spawner != null)
        {
            SetSpawner(spawner);
        }
    }

    public int GetSpawnerId() =>
        spawner != null ? spawner.GetSpawnerId() : -1;
}
