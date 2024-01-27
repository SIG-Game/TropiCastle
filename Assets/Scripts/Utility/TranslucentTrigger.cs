using UnityEngine;

public class TranslucentTrigger : MonoBehaviour
{
    [SerializeField] private SpriteRendererAlphaInterpolator alphaInterpolator;
    [SerializeField] private float translucentAlpha;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            alphaInterpolator.TargetAlpha = translucentAlpha;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            alphaInterpolator.TargetAlpha = 1f;
        }
    }
}
