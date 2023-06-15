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
        transform.position = new Vector3(GetNewXPosition(),
            GetNewYPosition(), transform.position.z);
    }

    private float GetNewXPosition()
    {
        float halfCameraWidth = camera.orthographicSize * camera.aspect;

        return GetNewPositionOnOneAxis(target.position.x,
            tilemapMinX, tilemapMaxX, halfCameraWidth);
    }

    private float GetNewYPosition()
    {
        float halfCameraHeight = camera.orthographicSize;

        return GetNewPositionOnOneAxis(target.position.y,
            tilemapMinY, tilemapMaxY, halfCameraHeight);
    }

    private float GetNewPositionOnOneAxis(float targetPosition,
        float tilemapMin, float tilemapMax, float halfCameraDimension)
    {
        float cameraMin = tilemapMin + halfCameraDimension;
        float cameraMax = tilemapMax - halfCameraDimension;

        return Mathf.Clamp(targetPosition, cameraMin, cameraMax);
    }
}
