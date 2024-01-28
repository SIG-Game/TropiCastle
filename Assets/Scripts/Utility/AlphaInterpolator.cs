using UnityEngine;

public abstract class AlphaInterpolator : MonoBehaviour
{
    [SerializeField] private float alphaChangeSpeed;

    protected abstract float Alpha { get; set; }

    public float TargetAlpha { private get; set; }

    protected virtual void Awake()
    {
        TargetAlpha = Alpha;
    }

    protected virtual void Update()
    {
        if (Alpha != TargetAlpha)
        {
            Alpha = Mathf.MoveTowards(Alpha, TargetAlpha,
                alphaChangeSpeed * Time.deltaTime);
        }
    }
}
