using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Sprite defaultCursorSprite;
    [SerializeField] private GameObject cursorBackground;
    [SerializeField] private Camera cursorCamera;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float gamepadSensitivity;

    public Sprite Sprite
    {
        get => cursorSpriteRenderer.sprite;
        set
        {
            cursorSpriteRenderer.sprite = value;
        }
    }

    private SpriteRenderer cursorSpriteRenderer;
    private SpriteRenderer cursorBackgroundSpriteRenderer;
    private InputAction moveCursorAction;
    private Vector2 cursorWorldPosition;
    private Vector2 horizontalCameraBounds;
    private Vector2 verticalCameraBounds;

    private void Awake()
    {
        HideAndLockMouseCursor();

        cursorSpriteRenderer = GetComponent<SpriteRenderer>();
        cursorBackgroundSpriteRenderer = cursorBackground.GetComponent<SpriteRenderer>();

        cursorWorldPosition = Vector2.zero;

        float halfCameraHeight = cursorCamera.orthographicSize;
        float halfCameraWidth = cursorCamera.orthographicSize * cursorCamera.aspect;

        horizontalCameraBounds = new Vector2(cursorCamera.transform.position.x - halfCameraWidth,
            cursorCamera.transform.position.x + halfCameraWidth);
        verticalCameraBounds = new Vector2(cursorCamera.transform.position.y - halfCameraHeight,
            cursorCamera.transform.position.y + halfCameraHeight);

        PauseController.OnGamePaused += PauseController_OnGamePaused;
        PauseController.OnGameUnpaused += PauseController_OnGameUnpaused;
        PlayerController.OnActionDisablingUIOpenSet += PlayerController_OnActionDisablingUIOpenSet;

        playerController.OnIsAttackingSet += PlayerController_OnIsAttackingSet;
    }

    private void Start()
    {
        moveCursorAction = InputManager.Instance.GetAction("Move Cursor");
    }

    // Needs to run before Update method in ItemPickupAndPlacement.cs
    // because that script uses the position of the in-game cursor
    private void Update()
    {
        Vector2 moveCursorInput = moveCursorAction.ReadValue<Vector2>();

        if (moveCursorInput != Vector2.zero)
        {
            Vector2 cursorDelta = moveCursorInput;

            if (InputManager.Instance.GetCurrentControlScheme() == "Mouse and Keyboard")
            {
                // Movement distance using mouse delta is not multiplied by Time.deltaTime
                // because doing that would make the movement distance vary with the frame rate
                cursorDelta *= mouseSensitivity;
            }
            else
            {
                cursorDelta *= gamepadSensitivity * Time.deltaTime;
            }

            transform.position = ClampToScreen(transform.position + (Vector3)cursorDelta);

            UpdateCursorWorldPosition();
        }
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

    public void SetCursorBackgroundColor(Color cursorBackgroundColor)
    {
        cursorBackgroundSpriteRenderer.color = cursorBackgroundColor;
    }

    public void UseDefaultCursor()
    {
        Sprite = defaultCursorSprite;
        SetCursorBackgroundColor(Color.clear);
    }

    public void SetCursorBackgroundLocalScale(Vector3 localScale)
    {
        cursorBackground.transform.localScale = localScale;
    }

    private Vector3 ClampToScreen(Vector3 input)
    {
        Vector2 clampedInput = new Vector3(
            Mathf.Clamp(input.x, horizontalCameraBounds.x, horizontalCameraBounds.y),
            Mathf.Clamp(input.y, verticalCameraBounds.x, verticalCameraBounds.y),
            input.z);

        return clampedInput;
    }

    private void UpdateCursorWorldPosition()
    {
        cursorWorldPosition = Camera.main.transform.position + transform.position;
    }

    private void HideAndLockMouseCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ShowAndUnlockMouseCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void PauseController_OnGamePaused()
    {
        ShowAndUnlockMouseCursor();

        gameObject.SetActive(false);
    }

    private void PauseController_OnGameUnpaused()
    {
        HideAndLockMouseCursor();

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

    public Vector2 GetWorldPosition() => cursorWorldPosition;
}
