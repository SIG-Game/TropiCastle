using UnityEngine;

public class ThrownItemWorld : MonoBehaviour
{
    private Rigidbody2D spawnedRigidbody2D;
    private Collider2D spawnedCollider2D;
    private WeaponController spawnedWeaponController;

    private const float speed = 5f;
    private const float enemyKnockbackForce = 2.5f;
    private const float maxStopVelocitySqrMagnitude = 0.001f;

    private void Awake()
    {
        spawnedRigidbody2D = GetComponent<Rigidbody2D>();
        spawnedCollider2D = GetComponent<Collider2D>();

        spawnedRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        spawnedRigidbody2D.gravityScale = 0f;
        spawnedRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        spawnedCollider2D.isTrigger = true;

        spawnedWeaponController = gameObject.AddComponent<WeaponController>();

        spawnedWeaponController.EnemyKnockbackForce = enemyKnockbackForce;
    }

    private void Update()
    {
        if (spawnedRigidbody2D.velocity.sqrMagnitude <= maxStopVelocitySqrMagnitude)
        {
            spawnedRigidbody2D.bodyType = RigidbodyType2D.Static;
            spawnedRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            spawnedCollider2D.isTrigger = false;

            Destroy(spawnedWeaponController);
            Destroy(this);
        }
    }

    public void SetUpThrownItemWorld(Vector3 velocityDirection)
    {
        spawnedRigidbody2D.velocity = speed * velocityDirection;
    }

    public void SetDamage(int damage)
    {
        spawnedWeaponController.Damage = damage;
    }
}
