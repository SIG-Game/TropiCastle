using UnityEngine;

public class RangedEnemyController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float targetRange;
    [SerializeField] private float waitTimeBeforeShootingSeconds;
    [SerializeField] private float timeBetweenShotsSeconds;

    private float shootTime;

    private void Awake()
    {
        shootTime = waitTimeBeforeShootingSeconds;
    }

    private void Update()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        transform.up = directionToPlayer;

        if (Vector3.Distance(transform.position, player.position) <= targetRange &&
            Time.time >= shootTime)
        {
            Instantiate(bullet, transform.position, transform.rotation);
            shootTime = Time.time + timeBetweenShotsSeconds;
        }
    }
}
