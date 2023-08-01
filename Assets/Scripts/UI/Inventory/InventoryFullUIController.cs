using System.Collections;
using UnityEngine;

public class InventoryFullUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroupAlphaInterpolator canvasGroupAlphaInterpolator;
    [SerializeField] private float visibleTimeSeconds;

    private Coroutine startFadingOutAfterWaitCoroutineObject;
    private WaitForSeconds visibleWaitForSeconds;

    public static InventoryFullUIController Instance;

    private void Awake()
    {
        Instance = this;

        visibleWaitForSeconds = new WaitForSeconds(visibleTimeSeconds);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private IEnumerator StartFadingOutAfterWaitCoroutine()
    {
        yield return visibleWaitForSeconds;

        canvasGroupAlphaInterpolator.TargetAlpha = 0f;
    }

    public void ShowInventoryFullText()
    {
        if (startFadingOutAfterWaitCoroutineObject != null)
        {
            StopCoroutine(startFadingOutAfterWaitCoroutineObject);
        }

        canvasGroupAlphaInterpolator.TargetAlpha = 1f;

        startFadingOutAfterWaitCoroutineObject =
            StartCoroutine(StartFadingOutAfterWaitCoroutine());
    }
}
