using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private float minTimeBetweenHits;
    [SerializeField] private bool logOnHit;

    public int Damage { private get; set; }
    public float EnemyKnockbackForce { private get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        EnemyController hitEnemy = other.GetComponent<EnemyController>();

        if (hitEnemy != null && hitEnemy.LastHitTime + minTimeBetweenHits <= Time.time)
        {
            hitEnemy.LastHitTime = Time.time;

            other.GetComponent<HealthController>().DecreaseHealth(Damage);

            Vector2 directionToEnemy = (other.transform.position - transform.position).normalized;
            hitEnemy.ApplyKnockback(directionToEnemy, EnemyKnockbackForce);

            if (logOnHit)
            {
                Debug.Log($"Attacked enemy {other.name} for {Damage} damage with knockback {EnemyKnockbackForce}.");
            }
        }
    }
}
