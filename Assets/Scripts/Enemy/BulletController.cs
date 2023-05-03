using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.up);
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
