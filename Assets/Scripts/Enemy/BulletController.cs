using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        rb2d.velocity = speed * transform.up;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"{nameof(BulletController)} trigger hit player");
            other.GetComponent<HealthController>().DecreaseHealth(10);
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
