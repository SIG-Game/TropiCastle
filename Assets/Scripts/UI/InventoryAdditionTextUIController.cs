using System.Collections;
using TMPro;
using UnityEngine;

public class InventoryAdditionTextUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inventoryAdditionText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float waitTimeBeforeFadeOutSeconds;
    [SerializeField] private float alphaChangeSpeed;

    private WaitForSeconds beforeFadeOutWaitForSeconds;
    private float targetAlpha;

    private void Awake()
    {
        beforeFadeOutWaitForSeconds = new WaitForSeconds(waitTimeBeforeFadeOutSeconds);

        StartCoroutine(WaitThenStartFadingOut());

        targetAlpha = 1f;
    }

    private void Update()
    {
        if (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha,
                alphaChangeSpeed * Time.deltaTime);

            if (canvasGroup.alpha == 0f && canvasGroup.alpha == targetAlpha)
            {
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator WaitThenStartFadingOut()
    {
        yield return beforeFadeOutWaitForSeconds;

        targetAlpha = 0f;
    }

    public void SetText(string text)
    {
        inventoryAdditionText.text = text;
    }
}
