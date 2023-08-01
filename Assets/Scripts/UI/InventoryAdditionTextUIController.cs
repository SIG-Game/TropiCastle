using System.Collections;
using TMPro;
using UnityEngine;

public class InventoryAdditionTextUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inventoryAdditionText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroupAlphaInterpolator canvasGroupAlphaInterpolator;
    [SerializeField] private float waitTimeBeforeFadeOutSeconds;

    private InventoryAdditionTextUISpawner spawner;
    private Coroutine waitThenStartFadingOutCoroutineObject;
    private WaitForSeconds beforeFadeOutWaitForSeconds;
    private string itemName;
    private int itemAmount;

    private void Awake()
    {
        beforeFadeOutWaitForSeconds =
            new WaitForSeconds(waitTimeBeforeFadeOutSeconds);

        waitThenStartFadingOutCoroutineObject =
            StartCoroutine(WaitThenStartFadingOutCoroutine());

        canvasGroupAlphaInterpolator.TargetAlpha = 1f;
    }

    // Must run before CanvasGroupAlphaInterpolator Update method to
    // prevent gameObject from being destroyed on the frame that
    // canvasGroupAlphaInterpolator.TargetAlpha is set to 0f
    private void Update()
    {
        bool fadedOut = canvasGroup.alpha == 0f &&
            canvasGroup.alpha == canvasGroupAlphaInterpolator.TargetAlpha;
        if (fadedOut)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        spawner.OnAdditionTextDestroyed(itemName);
    }

    private IEnumerator WaitThenStartFadingOutCoroutine()
    {
        yield return beforeFadeOutWaitForSeconds;

        canvasGroupAlphaInterpolator.TargetAlpha = 0f;
    }

    public void UpdateWithItem(ItemWithAmount item)
    {
        itemName = item.itemData.name;
        itemAmount = item.amount;

        UpdateText();
    }

    public void AddAmount(int amount)
    {
        itemAmount += amount;

        UpdateText();

        ResetFadeOut();
    }

    public void RemoveAmount(int amount)
    {
        itemAmount -= amount;

        if (itemAmount < 0)
        {
            itemAmount = 0;
        }

        UpdateText();
    }

    private void UpdateText()
    {
        inventoryAdditionText.text = $"+{itemAmount} {itemName}";
    }

    private void ResetFadeOut()
    {
        if (waitThenStartFadingOutCoroutineObject != null)
        {
            StopCoroutine(waitThenStartFadingOutCoroutineObject);
        }

        waitThenStartFadingOutCoroutineObject =
            StartCoroutine(WaitThenStartFadingOutCoroutine());

        canvasGroupAlphaInterpolator.TargetAlpha = 1f;
    }

    public void SetSpawner(InventoryAdditionTextUISpawner spawner)
    {
        this.spawner = spawner;
    }
}
