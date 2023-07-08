using System.Collections;
using UnityEngine;

public class PlayerOverlayController : MonoBehaviour
{
    [SerializeField] private HealthController playerHealthController;
    [SerializeField] private float maxAlpha;
    [SerializeField] private float timeAtMaxAlphaSeconds;
    [SerializeField] private float alphaChangeSpeed;

    private SpriteRenderer spriteRenderer;
    private Coroutine startFadingOutAfterWaitCoroutine;
    private WaitForSeconds maxAlphaWaitForSeconds;
    private float targetAlpha;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        maxAlphaWaitForSeconds = new WaitForSeconds(timeAtMaxAlphaSeconds);

        targetAlpha = 0f;

        playerHealthController.OnHealthChangedByAmount +=
            HealthController_OnHealthChangedByAmount;
    }

    private void Update()
    {
        if (spriteRenderer.color.a != targetAlpha)
        {
            float newAlpha = Mathf.MoveTowards(spriteRenderer.color.a,
                targetAlpha, alphaChangeSpeed * Time.deltaTime);

            SetAlpha(newAlpha);
        }
    }

    private void OnDestroy()
    {
        playerHealthController.OnHealthChangedByAmount -=
            HealthController_OnHealthChangedByAmount;
    }

    private IEnumerator StartFadingOutAfterWaitCoroutine()
    {
        yield return maxAlphaWaitForSeconds;

        targetAlpha = 0f;
    }

    private void SetAlpha(float alpha)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, alpha);
    }

    private void HealthController_OnHealthChangedByAmount(int healthDelta)
    {
        if (healthDelta < 0)
        {
            if (startFadingOutAfterWaitCoroutine != null)
            {
                StopCoroutine(startFadingOutAfterWaitCoroutine);
            }

            SetAlpha(maxAlpha);

            targetAlpha = maxAlpha;

            startFadingOutAfterWaitCoroutine =
                StartCoroutine(StartFadingOutAfterWaitCoroutine());
        }
    }
}
