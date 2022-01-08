using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float followXDist;
    public float followYDist;

    void LateUpdate()
    {
        float xDist = target.position.x - transform.position.x;
        float yDist = target.position.y - transform.position.y;

        if (xDist > followXDist)
            transform.position = new Vector3(target.position.x - followXDist, transform.position.y, transform.position.z);
        else if (xDist < -followXDist)
            transform.position = new Vector3(target.position.x + followXDist, transform.position.y, transform.position.z);

        if (yDist > followYDist)
            transform.position = new Vector3(transform.position.x, target.position.y - followYDist, transform.position.z);
        else if (yDist < -followYDist)
            transform.position = new Vector3(transform.position.x, target.position.y + followYDist, transform.position.z);
    }
}
