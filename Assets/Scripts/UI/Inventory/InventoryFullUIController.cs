using System.Collections;
using UnityEngine;

public class InventoryFullUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float visibleTimeSeconds;
    [SerializeField] private float alphaChangeSpeed;

    private Coroutine startFadingOutAfterWaitCoroutineObject;
    private WaitForSeconds visibleWaitForSeconds;
    private float targetAlpha;

    public static InventoryFullUIController Instance;

    private void Awake()
    {
        Instance = this;

        visibleWaitForSeconds = new WaitForSeconds(visibleTimeSeconds);

        targetAlpha = 0f;
    }

    private void Update()
    {
        if (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha,
                alphaChangeSpeed * Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private IEnumerator StartFadingOutAfterWaitCoroutine()
    {
        yield return visibleWaitForSeconds;

        targetAlpha = 0f;
    }

    public void ShowInventoryFullText()
    {
        if (startFadingOutAfterWaitCoroutineObject != null)
        {
            StopCoroutine(startFadingOutAfterWaitCoroutineObject);
        }

        targetAlpha = 1f;

        startFadingOutAfterWaitCoroutineObject =
            StartCoroutine(StartFadingOutAfterWaitCoroutine());
    }
}
