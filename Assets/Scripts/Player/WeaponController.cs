using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private float minTimeBetweenHits;
    [SerializeField] private float enemyKnockbackForce;

    public int damage { private get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        Enemy hitEnemy = other.GetComponent<Enemy>();

        if (hitEnemy != null && hitEnemy.LastHitTime + minTimeBetweenHits <= Time.time)
        {
            hitEnemy.LastHitTime = Time.time;

            other.GetComponent<HealthController>().DecreaseHealth(damage);

            Vector2 directionToEnemy = (other.transform.position - transform.position).normalized;
            hitEnemy.ApplyKnockback(directionToEnemy, enemyKnockbackForce);

            Debug.Log($"Attacked enemy {other.name} for {damage} damage.");
        }
    }
}
