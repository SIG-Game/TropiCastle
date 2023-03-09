using System;
using UnityEngine;

public static class SpawnColliderHelper
{
    public static bool TryGetSpawnPositionOutsideColliders(Func<Vector2> spawnPositionGenerator,
        Vector2 colliderExtents, int maxSpawnAttempts, out Vector2 spawnPosition)
    {
        bool canSpawn;
        int spawnAttempts = 0;

        do
        {
            spawnPosition = spawnPositionGenerator();

            canSpawn = CanSpawnColliderAtPosition(spawnPosition, colliderExtents);

            spawnAttempts++;

            if (spawnAttempts == maxSpawnAttempts && !canSpawn)
            {
                Debug.LogWarning($"Failed to generate spawn position outside of " +
                    $"colliders after {maxSpawnAttempts} attempts");

                return false;
            }
        } while (!canSpawn);

        return true;
    }

    private static bool CanSpawnColliderAtPosition(Vector2 position, Vector2 colliderExtents)
    {
        Vector2 overlapAreaCornerBottomLeft = position - colliderExtents;
        Vector2 overlapAreaCornerTopRight = position + colliderExtents;

        Collider2D colliderOverlap = Physics2D.OverlapArea(overlapAreaCornerBottomLeft, overlapAreaCornerTopRight);

        return colliderOverlap == null;
    }
}
