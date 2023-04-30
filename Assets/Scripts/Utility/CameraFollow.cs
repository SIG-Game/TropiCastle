using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Tilemap tilemap;

    public static event Action<Vector3> OnCameraMovedBy = delegate { };

    private new Camera camera;

    private float tilemapMinX;
    private float tilemapMaxX;
    private float tilemapMinY;
    private float tilemapMaxY;

    private void Start()
    {
        camera = GetComponent<Camera>();

        tilemapMinX = tilemap.localBounds.min.x;
        tilemapMaxX = tilemap.localBounds.max.x;
        tilemapMinY = tilemap.localBounds.min.y;
        tilemapMaxY = tilemap.localBounds.max.y;
    }

    private void LateUpdate()
    {
        float halfCameraHeight = camera.orthographicSize;
        float halfCameraWidth = camera.orthographicSize * camera.aspect;

        float minCameraX = tilemapMinX + halfCameraWidth;
        float maxCameraX = tilemapMaxX - halfCameraWidth;

        float minCameraY = tilemapMinY + halfCameraHeight;
        float maxCameraY = tilemapMaxY - halfCameraHeight;

        float newXPosition = Mathf.Clamp(target.position.x, minCameraX, maxCameraX);
        float newYPosition = Mathf.Clamp(target.position.y, minCameraY, maxCameraY);

        Vector3 newPosition = new Vector3(newXPosition, newYPosition, transform.position.z);

        Vector3 cameraPositionDelta = newPosition - transform.position;

        if (cameraPositionDelta != Vector3.zero)
        {
            OnCameraMovedBy(cameraPositionDelta);
        }

        transform.position = newPosition;
    }

    private void OnDestroy()
    {
        OnCameraMovedBy = delegate { };
    }
}
