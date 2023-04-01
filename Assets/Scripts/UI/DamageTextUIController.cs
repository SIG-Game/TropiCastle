using System.Collections;
using TMPro;
using UnityEngine;

public class DamageTextUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text damageText;
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

        while (damageText.color.a > 0f)
        {
            yield return null;

            float newAlpha = damageText.color.a - fadeOutSpeed * Time.deltaTime;

            if (newAlpha <= 0f)
            {
                newAlpha = 0f;

                Destroy(gameObject);
            }

            damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, newAlpha);
        }
    }

    public void SetUpDamageText(int damage)
    {
        damageText.text = $"-{damage}";
    }
}
