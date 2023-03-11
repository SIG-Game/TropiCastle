using UnityEngine;

public class InventoryFullUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float visibleTimeSeconds;
    [SerializeField] private float alphaChangeSpeed;

    private float visibleTimer;
    private float targetAlpha;

    public static InventoryFullUIController Instance;

    private void Awake()
    {
        Instance = this;

        targetAlpha = 0f;

        // Do not increase timer until inventory full UI is shown
        visibleTimer = visibleTimeSeconds;
    }

    private void Update()
    {
        if (visibleTimer < visibleTimeSeconds)
        {
            visibleTimer += Time.deltaTime;

            if (visibleTimer >= visibleTimeSeconds)
            {
                targetAlpha = 0f;
            }
        }

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

    public void ShowInventoryFullText()
    {
        visibleTimer = 0f;
        targetAlpha = 1f;
    }
}
