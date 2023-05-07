using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthChangeTextUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text healthChangeText;
    [SerializeField] private RectTransform backgroundRectTransform;
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    [SerializeField] private Color damageTextColor;
    [SerializeField] private Color healTextColor;
    [SerializeField] private float yVelocity;
    [SerializeField] private float waitSecondsBeforeFadeOut;
    [SerializeField] private float fadeOutSpeed;

    private WaitForSeconds beforeFadeOutWaitForSecondsObject;
    private float initialBackgroundAlpha;

    private void Awake()
    {
        beforeFadeOutWaitForSecondsObject = new WaitForSeconds(waitSecondsBeforeFadeOut);

        initialBackgroundAlpha = backgroundSpriteRenderer.color.a;

        StartCoroutine(WaitThenFadeOut());
    }

    private void Start()
    {
        // This method call doesn't work in the Awake method
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            healthChangeText.GetComponent<RectTransform>());

        backgroundRectTransform.localScale = new Vector3(
            healthChangeText.preferredWidth,
            backgroundRectTransform.localScale.y,
            backgroundRectTransform.localScale.z);
    }

    private void Update()
    {
        float newYPosition = transform.position.y + yVelocity * Time.deltaTime;

        transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
    }

    private IEnumerator WaitThenFadeOut()
    {
        yield return beforeFadeOutWaitForSecondsObject;

        float backgroundFadeOutSpeed = fadeOutSpeed * initialBackgroundAlpha;

        while (healthChangeText.color.a > 0f)
        {
            yield return null;

            float newAlpha = healthChangeText.color.a - fadeOutSpeed * Time.deltaTime;
            float newBackgroundAlpha = backgroundSpriteRenderer.color.a -
                backgroundFadeOutSpeed * Time.deltaTime;

            if (newAlpha <= 0f)
            {
                newAlpha = 0f;
                newBackgroundAlpha = 0f;

                Destroy(gameObject);
            }

            healthChangeText.color = new Color(healthChangeText.color.r, healthChangeText.color.g,
                healthChangeText.color.b, newAlpha);
            backgroundSpriteRenderer.color = new Color(backgroundSpriteRenderer.color.r,
                backgroundSpriteRenderer.color.g, backgroundSpriteRenderer.color.b,
                newBackgroundAlpha);
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
