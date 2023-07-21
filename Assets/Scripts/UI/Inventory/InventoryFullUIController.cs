using System.Collections;
using UnityEngine;

public class InventoryFullUIController : CanvasGroupAlphaInterpolator
{
    [SerializeField] private float visibleTimeSeconds;

    private Coroutine startFadingOutAfterWaitCoroutineObject;
    private WaitForSeconds visibleWaitForSeconds;

    public static InventoryFullUIController Instance;

    protected override void Awake()
    {
        base.Awake();

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

        TargetAlpha = 0f;
    }

    public void ShowInventoryFullText()
    {
        if (startFadingOutAfterWaitCoroutineObject != null)
        {
            StopCoroutine(startFadingOutAfterWaitCoroutineObject);
        }

        TargetAlpha = 1f;

        startFadingOutAfterWaitCoroutineObject =
            StartCoroutine(StartFadingOutAfterWaitCoroutine());
    }
}
