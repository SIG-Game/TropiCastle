using UnityEngine;

public class DamageTextUISpawner : MonoBehaviour
{
    [SerializeField] private HealthController targetHealthController;
    [SerializeField] private GameObject damageTextPrefab;

    private int? previousHealth;

    private void Awake()
    {
        targetHealthController.OnHealthChanged += HealthController_OnHealthChanged;
    }

    private void OnDestroy()
    {
        targetHealthController.OnHealthChanged -= HealthController_OnHealthChanged;
    }

    private void HealthController_OnHealthChanged(int newHealth)
    {
        if (previousHealth != null)
        {
            int damage = previousHealth.Value - newHealth;

            if (damage > 0)
            {
                GameObject damageTextGameObject = Instantiate(damageTextPrefab, transform);

                damageTextGameObject.GetComponent<DamageTextUIController>().SetUpDamageText(damage);
            }
        }

        previousHealth = newHealth;
    }
}
