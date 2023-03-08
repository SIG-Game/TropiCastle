using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Sprite defaultCursorSprite;
    [SerializeField] private GameObject cursorBackground;

    private SpriteRenderer cursorSpriteRenderer;
    private SpriteRenderer cursorBackgroundSpriteRenderer;

    private void Awake()
    {
        Cursor.visible = false;

        cursorSpriteRenderer = GetComponent<SpriteRenderer>();
        cursorBackgroundSpriteRenderer = cursorBackground.GetComponent<SpriteRenderer>();

        PauseController.OnGamePaused += PauseController_OnGamePaused;
        PauseController.OnGameUnpaused += PauseController_OnGameUnpaused;
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            return;
        }

        Vector2 mousePositionClampedToScreen = new Vector2(
            Mathf.Clamp(Input.mousePosition.x, 0f, Screen.width),
            Mathf.Clamp(Input.mousePosition.y, 0f, Screen.height));

        Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(mousePositionClampedToScreen);

        transform.position = mouseWorldPoint;
    }

    private void OnDestroy()
    {
        PauseController.OnGamePaused -= PauseController_OnGamePaused;
        PauseController.OnGameUnpaused -= PauseController_OnGameUnpaused;
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

    private void HideCursor()
    {
        cursorSpriteRenderer.sprite = null;
        cursorBackgroundSpriteRenderer.color = Color.clear;
    }

    private void PauseController_OnGamePaused()
    {
        Cursor.visible = true;
        HideCursor();
    }

    private void PauseController_OnGameUnpaused()
    {
        Cursor.visible = false;
        UseDefaultCursor();
    }
}
