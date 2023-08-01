using UnityEngine;

public class CanvasGroupAlphaInterpolator : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float alphaChangeSpeed;

    public float TargetAlpha { get; set; }

    private void Awake()
    {
        TargetAlpha = canvasGroup.alpha;
    }

    private void Update()
    {
        if (canvasGroup.alpha != TargetAlpha)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha,
                TargetAlpha, alphaChangeSpeed * Time.deltaTime);
        }
    }
}
