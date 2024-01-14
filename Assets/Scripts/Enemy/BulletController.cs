using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float speed;

    private new Rigidbody2D rigidbody2D;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        rigidbody2D.velocity = speed * transform.up;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthController>().Health -= 10;

            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
