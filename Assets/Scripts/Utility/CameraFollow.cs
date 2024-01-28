using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Tilemap boundingTilemap;

    private new Camera camera;

    private float tilemapMinX;
    private float tilemapMaxX;
    private float tilemapMinY;
    private float tilemapMaxY;

    private void Start()
    {
        camera = GetComponent<Camera>();

        tilemapMinX = boundingTilemap.localBounds.min.x;
        tilemapMaxX = boundingTilemap.localBounds.max.x;
        tilemapMinY = boundingTilemap.localBounds.min.y;
        tilemapMaxY = boundingTilemap.localBounds.max.y;
    }

    private void Update()
    {
        float halfCameraHeight = camera.orthographicSize;
        float halfCameraWidth = halfCameraHeight * camera.aspect;

        float newXPosition = GetNewPositionOnOneAxis(
            target.position.x, tilemapMinX, tilemapMaxX, halfCameraWidth);
        float newYPosition = GetNewPositionOnOneAxis(
            target.position.y, tilemapMinY, tilemapMaxY, halfCameraHeight);

        transform.position = new Vector3(
            newXPosition, newYPosition, transform.position.z);
    }

    private float GetNewPositionOnOneAxis(float targetPosition,
        float tilemapMin, float tilemapMax, float halfCameraDimension)
    {
        float cameraMin = tilemapMin + halfCameraDimension;
        float cameraMax = tilemapMax - halfCameraDimension;

        return Mathf.Clamp(targetPosition, cameraMin, cameraMax);
    }
}
