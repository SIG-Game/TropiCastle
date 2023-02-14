using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Sprite defaultCursorSprite;
    [SerializeField] private GameObject cursorBackground;

    private SpriteRenderer cursorSpriteRenderer;
    private SpriteRenderer cursorBackgroundSpriteRenderer;

    private void Awake()
    {
        cursorSpriteRenderer = GetComponent<SpriteRenderer>();
        cursorBackgroundSpriteRenderer = cursorBackground.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            UseDefaultCursor();
        }

        Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mouseWorldPoint;
    }

    public void SetCursorSprite(Sprite sprite)
    {
        cursorSpriteRenderer.sprite = sprite;
    }

    public void SetCursorBackgroundColor(Color cursorBackgroundColor)
    {
        cursorBackgroundSpriteRenderer.color = cursorBackgroundColor;
    }

    public void UseDefaultCursor()
    {
        cursorSpriteRenderer.sprite = defaultCursorSprite;
        cursorBackgroundSpriteRenderer.color = Color.clear;
    }

    public void SetCursorBackgroundLocalScale(Vector3 localScale)
    {
        cursorBackground.transform.localScale = localScale;
    }
}
