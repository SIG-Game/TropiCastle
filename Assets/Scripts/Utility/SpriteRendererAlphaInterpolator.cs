using UnityEngine;

public class SpriteRendererAlphaInterpolator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float alphaChangeSpeed;

    [Inject] private PauseController pauseController;

    public float TargetAlpha { private get; set; }

    private void Awake()
    {
        this.InjectDependencies();

        TargetAlpha = spriteRenderer.color.a;
    }

    private void Update()
    {
        if (!pauseController.GamePaused &&
            spriteRenderer.color.a != TargetAlpha)
        {
            float newAlpha = Mathf.MoveTowards(spriteRenderer.color.a,
                TargetAlpha, alphaChangeSpeed * Time.deltaTime);

            spriteRenderer.color = new Color(
                spriteRenderer.color.r,
                spriteRenderer.color.g,
                spriteRenderer.color.b,
                newAlpha);
        }
    }
}
