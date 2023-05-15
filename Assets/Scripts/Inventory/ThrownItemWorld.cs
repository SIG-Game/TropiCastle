using System.Collections;
using UnityEngine;

public class ThrownItemWorld : MonoBehaviour
{
    private Rigidbody2D spawnedRigidbody2D;
    private Collider2D spawnedCollider2D;
    private WeaponController spawnedWeaponController;
    private WaitForSeconds beforeStopWaitForSeconds;

    private const float waitTimeBeforeStop = 0.3f;
    private const float speed = 5f;
    private const float enemyKnockbackForce = 2.5f;

    private void Awake()
    {
        spawnedRigidbody2D = GetComponent<Rigidbody2D>();
        spawnedCollider2D = GetComponent<Collider2D>();

        beforeStopWaitForSeconds = new WaitForSeconds(waitTimeBeforeStop);

        spawnedRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        spawnedRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        spawnedCollider2D.isTrigger = true;

        spawnedWeaponController = gameObject.AddComponent<WeaponController>();

        spawnedWeaponController.EnemyKnockbackForce = enemyKnockbackForce;
    }

    public void SetUpThrownItemWorld(Vector3 velocityDirection)
    {
        spawnedRigidbody2D.velocity = speed * velocityDirection;

        StartCoroutine(ThrowCoroutine());
    }

    private IEnumerator ThrowCoroutine()
    {
        yield return beforeStopWaitForSeconds;

        spawnedRigidbody2D.bodyType = RigidbodyType2D.Static;
        spawnedRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        spawnedCollider2D.isTrigger = false;

        Destroy(spawnedWeaponController);
    }

    public void SetDamage(int damage)
    {
        spawnedWeaponController.Damage = damage;
    }
}
