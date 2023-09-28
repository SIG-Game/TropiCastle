using UnityEngine;

public class SpawnAreaVisualizer : MonoBehaviour
{
    [SerializeField] private Color drawnSpawnAreaColor = Color.black;

    private PrefabSpawner spawner;
    private Vector3 topLeftCornerSpawnArea;
    private Vector3 topRightCornerSpawnArea;
    private Vector3 bottomLeftCornerSpawnArea;
    private Vector3 bottomRightCornerSpawnArea;

    private void OnEnable()
    {
        spawner = GetComponent<PrefabSpawner>();

        Vector3 minSpawnPosition = spawner.GetMinSpawnPosition();
        Vector3 maxSpawnPosition = spawner.GetMaxSpawnPosition();

        topLeftCornerSpawnArea = new Vector3(minSpawnPosition.x, maxSpawnPosition.y);
        topRightCornerSpawnArea = new Vector3(maxSpawnPosition.x, maxSpawnPosition.y);
        bottomLeftCornerSpawnArea = new Vector3(minSpawnPosition.x, minSpawnPosition.y);
        bottomRightCornerSpawnArea = new Vector3(maxSpawnPosition.x, minSpawnPosition.y);
    }

    private void Update()
    {
        Debug.DrawLine(topLeftCornerSpawnArea, topRightCornerSpawnArea, drawnSpawnAreaColor);
        Debug.DrawLine(topRightCornerSpawnArea, bottomRightCornerSpawnArea, drawnSpawnAreaColor);
        Debug.DrawLine(bottomRightCornerSpawnArea, bottomLeftCornerSpawnArea, drawnSpawnAreaColor);
        Debug.DrawLine(bottomLeftCornerSpawnArea, topLeftCornerSpawnArea, drawnSpawnAreaColor);
    }
}
