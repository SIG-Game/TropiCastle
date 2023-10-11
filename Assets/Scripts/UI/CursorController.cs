using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Sprite defaultCursorSprite;
    [SerializeField] private GameObject cursorBackground;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Camera cursorCamera;
    [SerializeField] private PauseController pauseController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerActionDisablingUIManager playerActionDisablingUIManager;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionReference moveCursorActionReference;
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
    private float previousCameraOrthographicSize;
    private float previousCameraAspect;

    private void Awake()
    {
        HideAndLockMouseCursor();

        cursorSpriteRenderer = GetComponent<SpriteRenderer>();
        cursorBackgroundSpriteRenderer = cursorBackground.GetComponent<SpriteRenderer>();

        moveCursorAction = moveCursorActionReference.action;

        cursorWorldPosition = Vector2.zero;

        pauseController.OnGamePausedSet += PauseController_OnGamePausedSet;
        playerActionDisablingUIManager.OnUIOpened += HideIfUnpaused;
        playerActionDisablingUIManager.OnUIClosed += ShowIfUnpaused;
        playerController.OnIsAttackingSet += PlayerController_OnIsAttackingSet;
    }

    // Needs to run before Update method in ItemPickupAndPlacement.cs
    // because that script uses the position of the in-game cursor
    private void Update()
    {
        bool cameraSizeChanged =
            cursorCamera.orthographicSize != previousCameraOrthographicSize ||
            cursorCamera.aspect != previousCameraAspect;
        if (cameraSizeChanged)
        {
            UpdateCameraBounds();
        }

        Vector2 moveCursorInput = moveCursorAction.ReadValue<Vector2>();

        if (moveCursorInput != Vector2.zero)
        {
            Vector2 cursorDelta = moveCursorInput;

            if (playerInput.currentControlScheme == "Mouse and Keyboard")
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
        else if (cameraSizeChanged)
        {
            transform.position = ClampToScreen(transform.position);
        }

        UpdateCursorWorldPosition();

        previousCameraOrthographicSize = cursorCamera.orthographicSize;
        previousCameraAspect = cursorCamera.aspect;
    }

    private void OnDestroy()
    {
        pauseController.OnGamePausedSet -= PauseController_OnGamePausedSet;
        playerActionDisablingUIManager.OnUIOpened -= HideIfUnpaused;
        playerActionDisablingUIManager.OnUIClosed -= ShowIfUnpaused;

        if (playerController != null)
        {
            playerController.OnIsAttackingSet -= PlayerController_OnIsAttackingSet;
        }
    }

    public void UpdateUsingItem(ItemWithAmount item)
    {
        Sprite = item.itemDefinition.sprite;
        amountText.text = item.GetAmountText();
    }

    public void UpdateCursorBackground(Color backgroundColor,
        Vector3 backgroundLocalScale)
    {
        cursorBackgroundSpriteRenderer.color = backgroundColor;
        cursorBackground.transform.localScale = backgroundLocalScale;
    }

    public void UseDefaultCursor()
    {
        Sprite = defaultCursorSprite;
        amountText.text = string.Empty;
        UpdateCursorBackground(Color.clear, Vector3.zero);
    }

    private void UpdateCameraBounds()
    {
        float halfCameraHeight = cursorCamera.orthographicSize;
        float halfCameraWidth = cursorCamera.orthographicSize * cursorCamera.aspect;

        horizontalCameraBounds = new Vector2(cursorCamera.transform.position.x - halfCameraWidth,
            cursorCamera.transform.position.x + halfCameraWidth);
        verticalCameraBounds = new Vector2(cursorCamera.transform.position.y - halfCameraHeight,
            cursorCamera.transform.position.y + halfCameraHeight);
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

    private void HideIfUnpaused()
    {
        if (!PauseController.Instance.GamePaused)
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowIfUnpaused()
    {
        if (!PauseController.Instance.GamePaused)
        {
            gameObject.SetActive(true);
        }
    }

    private void PauseController_OnGamePausedSet()
    {
        if (PauseController.Instance.GamePaused)
        {
            ShowAndUnlockMouseCursor();

            gameObject.SetActive(false);
        }
        else
        {
            HideAndLockMouseCursor();

            if (!playerActionDisablingUIManager.ActionDisablingUIOpen &&
                !playerController.IsAttacking)
            {
                gameObject.SetActive(true);
            }
        }
    }

    private void PlayerController_OnIsAttackingSet(bool isAttacking)
    {
        if (PauseController.Instance.GamePaused)
        {
            return;
        }

        gameObject.SetActive(!isAttacking);
    }

    public Vector2 GetWorldPosition() => cursorWorldPosition;
}
