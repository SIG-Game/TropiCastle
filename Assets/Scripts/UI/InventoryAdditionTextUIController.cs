using System.Collections;
using TMPro;
using UnityEngine;

public class InventoryAdditionTextUIController : CanvasGroupAlphaInterpolator
{
    [SerializeField] private TextMeshProUGUI inventoryAdditionText;
    [SerializeField] private float waitTimeBeforeFadeOutSeconds;

    private InventoryAdditionTextUISpawner spawner;
    private Coroutine waitThenStartFadingOutCoroutineObject;
    private WaitForSeconds beforeFadeOutWaitForSeconds;
    private string itemName;
    private int itemAmount;

    protected override void Awake()
    {
        base.Awake();

        beforeFadeOutWaitForSeconds = new WaitForSeconds(waitTimeBeforeFadeOutSeconds);

        waitThenStartFadingOutCoroutineObject = StartCoroutine(WaitThenStartFadingOut());

        TargetAlpha = 1f;
    }

    protected override void Update()
    {
        base.Update();

        if (canvasGroup.alpha == 0f && canvasGroup.alpha == TargetAlpha)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        spawner.OnAdditionTextDestroyed(itemName);
    }

    private IEnumerator WaitThenStartFadingOut()
    {
        yield return beforeFadeOutWaitForSeconds;

        TargetAlpha = 0f;
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

        waitThenStartFadingOutCoroutineObject = StartCoroutine(WaitThenStartFadingOut());

        TargetAlpha = 1f;
    }

    public void SetSpawner(InventoryAdditionTextUISpawner spawner)
    {
        this.spawner = spawner;
    }
}
