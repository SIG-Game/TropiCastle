using System.Collections;
using TMPro;
using UnityEngine;

public class InventoryAdditionTextUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inventoryAdditionText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float waitTimeBeforeFadeOutSeconds;
    [SerializeField] private float alphaChangeSpeed;

    private InventoryAdditionTextUISpawner spawner;
    private Coroutine waitThenStartFadingOutCoroutineObject;
    private WaitForSeconds beforeFadeOutWaitForSeconds;
    private float targetAlpha;
    private string itemName;
    private int itemAmount;

    private void Awake()
    {
        beforeFadeOutWaitForSeconds = new WaitForSeconds(waitTimeBeforeFadeOutSeconds);

        waitThenStartFadingOutCoroutineObject = StartCoroutine(WaitThenStartFadingOut());

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

    private void OnDestroy()
    {
        spawner.OnAdditionTextDestroyed(itemName);
    }

    private IEnumerator WaitThenStartFadingOut()
    {
        yield return beforeFadeOutWaitForSeconds;

        targetAlpha = 0f;
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

        targetAlpha = 1f;
    }

    public void SetSpawner(InventoryAdditionTextUISpawner spawner)
    {
        this.spawner = spawner;
    }
}
