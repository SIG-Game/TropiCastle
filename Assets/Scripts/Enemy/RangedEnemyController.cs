using UnityEngine;

public class RangedEnemyController : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float targetRange;
    [SerializeField] private float waitTimeBeforeShootingSeconds;
    [SerializeField] private float timeBetweenShotsSeconds;

    [Inject("PlayerTransform")] private Transform playerTransform;

    private float shootTime;

    private void Awake()
    {
        this.InjectDependencies();

        shootTime = waitTimeBeforeShootingSeconds;
    }

    private void Update()
    {
        Vector2 directionToPlayer = playerTransform.position - transform.position;
        transform.up = directionToPlayer;

        if (Vector3.Distance(transform.position, playerTransform.position) <= targetRange &&
            Time.time >= shootTime)
        {
            Instantiate(bullet, transform.position, transform.rotation);
            shootTime = Time.time + timeBetweenShotsSeconds;
        }
    }
}
