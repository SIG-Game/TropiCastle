using System.Collections;
using UnityEngine;

public class ThrownCoconutItemWorld : MonoBehaviour
{
    private Rigidbody2D spawnedCoconutRigidbody2D;
    private Collider2D spawnedCoconutCollider2D;
    private WaitForSeconds beforeStopWaitForSeconds;

    private const float waitTimeBeforeStop = 0.3f;
    private const float speed = 5f;

    private void Awake()
    {
        spawnedCoconutRigidbody2D = GetComponent<Rigidbody2D>();
        spawnedCoconutCollider2D = GetComponent<Collider2D>();

        beforeStopWaitForSeconds = new WaitForSeconds(waitTimeBeforeStop);

        spawnedCoconutRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        spawnedCoconutRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        spawnedCoconutCollider2D.isTrigger = true;
    }

    public void SetUpThrownCoconutItemWorld(Vector3 velocityDirection)
    {
        spawnedCoconutRigidbody2D.velocity = speed * velocityDirection;

        StartCoroutine(ThrowCoroutine());
    }

    private IEnumerator ThrowCoroutine()
    {
        yield return beforeStopWaitForSeconds;

        spawnedCoconutRigidbody2D.bodyType = RigidbodyType2D.Static;
        spawnedCoconutRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        spawnedCoconutCollider2D.isTrigger = false;
    }
}
