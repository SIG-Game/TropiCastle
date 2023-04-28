using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Sprite defaultCursorSprite;
    [SerializeField] private GameObject cursorBackground;
    [SerializeField] private PlayerController playerController;

    private SpriteRenderer cursorSpriteRenderer;
    private SpriteRenderer cursorBackgroundSpriteRenderer;

    private void Awake()
    {
        Cursor.visible = false;

        cursorSpriteRenderer = GetComponent<SpriteRenderer>();
        cursorBackgroundSpriteRenderer = cursorBackground.GetComponent<SpriteRenderer>();

        PauseController.OnGamePaused += PauseController_OnGamePaused;
        PauseController.OnGameUnpaused += PauseController_OnGameUnpaused;
        PlayerController.OnActionDisablingUIOpenSet += PlayerController_OnActionDisablingUIOpenSet;

        playerController.OnIsAttackingSet += PlayerController_OnIsAttackingSet;
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused || PlayerController.ActionDisablingUIOpen ||
            playerController.IsAttacking)
        {
            return;
        }

        Vector2 mouseWorldPoint = Camera.main.ScreenToWorldPoint(
            MousePositionHelper.GetMousePositionClampedToScreen());

        transform.position = mouseWorldPoint;
    }

    private void OnDestroy()
    {
        PauseController.OnGamePaused -= PauseController_OnGamePaused;
        PauseController.OnGameUnpaused -= PauseController_OnGameUnpaused;
        PlayerController.OnActionDisablingUIOpenSet -= PlayerController_OnActionDisablingUIOpenSet;

        if (playerController != null)
        {
            playerController.OnIsAttackingSet -= PlayerController_OnIsAttackingSet;
        }
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

    private void PauseController_OnGamePaused()
    {
        Cursor.visible = true;

        gameObject.SetActive(false);
    }

    private void PauseController_OnGameUnpaused()
    {
        Cursor.visible = false;

        if (!PlayerController.ActionDisablingUIOpen)
        {
            gameObject.SetActive(true);
        }
    }

    private void PlayerController_OnActionDisablingUIOpenSet(bool actionDisablingUIOpen)
    {
        gameObject.SetActive(!actionDisablingUIOpen);
    }

    private void PlayerController_OnIsAttackingSet(bool isAttacking)
    {
        gameObject.SetActive(!isAttacking);
    }
}
