using UnityEngine;

public class ThrownItemWorld : MonoBehaviour
{
    private Rigidbody2D spawnedRigidbody2D;
    private Collider2D spawnedCollider2D;
    private WeaponController spawnedWeaponController;
    private Collider2D playerCollider2D;

    private const float stopDamagingMaxVelocitySqrMagnitude = 1f;
    private const float maxStopVelocitySqrMagnitude = 0.001f;

    private void Awake()
    {
        spawnedRigidbody2D = GetComponent<Rigidbody2D>();
        spawnedCollider2D = GetComponent<Collider2D>();

        spawnedRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        spawnedRigidbody2D.gravityScale = 0f;
        spawnedRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        spawnedWeaponController = gameObject.AddComponent<WeaponController>();
    }

    private void Update()
    {
        if (spawnedRigidbody2D.velocity.sqrMagnitude <= stopDamagingMaxVelocitySqrMagnitude)
        {
            Destroy(spawnedWeaponController);
        }

        if (spawnedRigidbody2D.velocity.sqrMagnitude <= maxStopVelocitySqrMagnitude)
        {
            spawnedRigidbody2D.bodyType = RigidbodyType2D.Static;
            spawnedRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

            Physics2D.IgnoreCollision(playerCollider2D, spawnedCollider2D, false);

            Destroy(this);
        }
    }

    public void SetUpThrownItemWorld(Vector3 velocityDirection,
        ThrowableItemScriptableObject throwableItemScriptableObject,
        Collider2D playerCollider2D)
    {
        spawnedRigidbody2D.velocity = throwableItemScriptableObject.speed * velocityDirection;

        spawnedWeaponController.Damage = throwableItemScriptableObject.damage;
        spawnedWeaponController.EnemyKnockbackForce = throwableItemScriptableObject.knockback;

        this.playerCollider2D = playerCollider2D;

        Physics2D.IgnoreCollision(playerCollider2D, spawnedCollider2D);
    }
}
