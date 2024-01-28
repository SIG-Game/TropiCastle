using UnityEngine;

public class SpriteRendererAlphaInterpolator : AlphaInterpolator
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Inject] private PauseController pauseController;

    protected override float Alpha
    {
        get => spriteRenderer.color.a;
        set
        {
            spriteRenderer.color = new Color(
                spriteRenderer.color.r,
                spriteRenderer.color.g,
                spriteRenderer.color.b,
                value);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();
    }

    protected override void Update()
    {
        if (!pauseController.GamePaused)
        {
            base.Update();
        }
    }
}
