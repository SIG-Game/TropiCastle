using System.Collections;
using TMPro;
using UnityEngine;

public class HealthChangeTextUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text healthChangeText;
    [SerializeField] private Color damageTextColor;
    [SerializeField] private Color healTextColor;
    [SerializeField] private float yVelocity;
    [SerializeField] private float waitSecondsBeforeFadeOut;
    [SerializeField] private float fadeOutSpeed;

    private WaitForSeconds beforeFadeOutWaitForSecondsObject;

    private void Awake()
    {
        beforeFadeOutWaitForSecondsObject = new WaitForSeconds(waitSecondsBeforeFadeOut);

        StartCoroutine(WaitThenFadeOut());
    }

    private void Update()
    {
        float newYPosition = transform.position.y + yVelocity * Time.deltaTime;

        transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
    }

    private IEnumerator WaitThenFadeOut()
    {
        yield return beforeFadeOutWaitForSecondsObject;

        while (healthChangeText.color.a > 0f)
        {
            yield return null;

            float newAlpha = healthChangeText.color.a - fadeOutSpeed * Time.deltaTime;

            if (newAlpha <= 0f)
            {
                newAlpha = 0f;

                Destroy(gameObject);
            }

            healthChangeText.color = new Color(healthChangeText.color.r, healthChangeText.color.g,
                healthChangeText.color.b, newAlpha);
        }
    }

    public void SetUpHealthChangeText(int healthDelta)
    {
        if (healthDelta < 0)
        {
            healthChangeText.text = healthDelta.ToString();
            healthChangeText.color = damageTextColor;
        }
        else
        {
            healthChangeText.text = $"+{healthDelta}";
            healthChangeText.color = healTextColor;
        }
    }
}
