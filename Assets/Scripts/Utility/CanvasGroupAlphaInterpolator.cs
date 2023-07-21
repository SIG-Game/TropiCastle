using UnityEngine;

public class CanvasGroupAlphaInterpolator : MonoBehaviour
{
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] private float alphaChangeSpeed;

    protected float TargetAlpha { get; set; }

    protected virtual void Awake()
    {
        TargetAlpha = canvasGroup.alpha;
    }

    protected virtual void Update()
    {
        if (canvasGroup.alpha != TargetAlpha)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha,
                TargetAlpha, alphaChangeSpeed * Time.deltaTime);
        }
    }
}
