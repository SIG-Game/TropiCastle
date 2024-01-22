using System.Collections;
using UnityEngine;

public class InventoryFullUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroupAlphaInterpolator canvasGroupAlphaInterpolator;
    [SerializeField] private float visibleTimeSeconds;

    [Inject("PlayerInventory")] private Inventory playerInventory;

    private Coroutine startFadingOutAfterWaitCoroutineObject;
    private WaitForSeconds visibleWaitForSeconds;

    private void Awake()
    {
        this.InjectDependencies();

        visibleWaitForSeconds = new WaitForSeconds(visibleTimeSeconds);

        playerInventory.OnFailedToAddItemToFullInventory +=
            Inventory_OnFailedToAddItemToFullInventory;
    }

    private void OnDestroy()
    {
        playerInventory.OnFailedToAddItemToFullInventory -=
            Inventory_OnFailedToAddItemToFullInventory;
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

    private void Inventory_OnFailedToAddItemToFullInventory()
    {
        ShowInventoryFullText();
    }
}
