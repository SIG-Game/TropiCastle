using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public int damage { private get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<HealthController>().DecreaseHealth(damage);

            Vector2 directionToEnemy = (other.transform.position - transform.position).normalized;
            other.GetComponent<Rigidbody2D>().AddForce(directionToEnemy, ForceMode2D.Impulse);

            Debug.Log($"Attacked enemy {other.name} for {damage} damage.");
        }
    }
}
