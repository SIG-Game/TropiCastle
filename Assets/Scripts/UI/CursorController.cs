using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Sprite defaultCursorSprite;
    [SerializeField] private GameObject cursorBackground;
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

    private void Awake()
    {
        HideAndLockMouseCursor();

        cursorSpriteRenderer = GetComponent<SpriteRenderer>();
        cursorBackgroundSpriteRenderer = cursorBackground.GetComponent<SpriteRenderer>();

        PauseController.OnGamePaused += PauseController_OnGamePaused;
        PauseController.OnGameUnpaused += PauseController_OnGameUnpaused;
        PlayerController.OnActionDisablingUIOpenSet += PlayerController_OnActionDisablingUIOpenSet;
        CameraFollow.OnCameraMovedBy += CameraFollow_OnCameraMovedBy;

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
        }
    }

    private void OnDestroy()
    {
        PauseController.OnGamePaused -= PauseController_OnGamePaused;
        PauseController.OnGameUnpaused -= PauseController_OnGameUnpaused;
        PlayerController.OnActionDisablingUIOpenSet -= PlayerController_OnActionDisablingUIOpenSet;
        CameraFollow.OnCameraMovedBy -= CameraFollow_OnCameraMovedBy;

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
        float halfCameraHeight = Camera.main.orthographicSize;
        float halfCameraWidth = Camera.main.orthographicSize * Camera.main.aspect;

        Vector2 horizontalCameraBounds = new Vector2(Camera.main.transform.position.x - halfCameraWidth,
            Camera.main.transform.position.x + halfCameraWidth);
        Vector2 verticalCameraBounds = new Vector2(Camera.main.transform.position.y - halfCameraHeight,
            Camera.main.transform.position.y + halfCameraHeight);

        Vector2 clampedInput = new Vector3(
            Mathf.Clamp(input.x, horizontalCameraBounds.x, horizontalCameraBounds.y),
            Mathf.Clamp(input.y, verticalCameraBounds.x, verticalCameraBounds.y),
            input.z);

        return clampedInput;
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

    private void CameraFollow_OnCameraMovedBy(Vector3 cameraPositionDelta)
    {
        transform.position = ClampToScreen(transform.position + cameraPositionDelta);
    }

    public Vector2 GetPosition() => transform.position;
}
