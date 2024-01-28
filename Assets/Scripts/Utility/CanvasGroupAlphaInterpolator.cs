using UnityEngine;

public class CanvasGroupAlphaInterpolator : AlphaInterpolator
{
    [SerializeField] private CanvasGroup canvasGroup;

    protected override float Alpha
    {
        get => canvasGroup.alpha;
        set => canvasGroup.alpha = value;
    }
}
