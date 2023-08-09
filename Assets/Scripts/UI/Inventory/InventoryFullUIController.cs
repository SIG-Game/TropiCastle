using System.Collections;
using UnityEngine;

public class InventoryFullUIController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private CanvasGroupAlphaInterpolator canvasGroupAlphaInterpolator;
    [SerializeField] private float visibleTimeSeconds;

    private Coroutine startFadingOutAfterWaitCoroutineObject;
    private WaitForSeconds visibleWaitForSeconds;

    private void Awake()
    {
        visibleWaitForSeconds = new WaitForSeconds(visibleTimeSeconds);

        inventory.OnFailedToAddItemToFullInventory +=
            Inventory_OnFailedToAddItemToFullInventory;
    }

    private void OnDestroy()
    {
        inventory.OnFailedToAddItemToFullInventory -=
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
