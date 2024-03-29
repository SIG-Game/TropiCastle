using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Sprite defaultCursorSprite;
    [SerializeField] private GameObject cursorBackground;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Camera cursorCamera;
    [SerializeField] private InputActionReference moveCursorActionReference;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float gamepadSensitivity;

    [Inject] private PauseController pauseController;
    [Inject] private PlayerActionDisablingUIManager playerActionDisablingUIManager;
    [Inject] private PlayerController playerController;
    [Inject] private PlayerInput playerInput;

    public Sprite Sprite
    {
        get => cursorSpriteRenderer.sprite;
        set
        {
            cursorSpriteRenderer.sprite = value;
        }
    }

    public bool LockToGrid { private get; set; }

    private SpriteRenderer cursorSpriteRenderer;
    private SpriteRenderer cursorBackgroundSpriteRenderer;
    private InputAction moveCursorAction;
    private Vector2 cursorPosition;
    private Vector2 cursorWorldPosition;
    private Vector2 horizontalCameraBounds;
    private Vector2 verticalCameraBounds;
    private float previousCameraOrthographicSize;
    private float previousCameraAspect;

    private void Awake()
    {
        this.InjectDependencies();

        HideAndLockMouseCursor();

        cursorSpriteRenderer = GetComponent<SpriteRenderer>();
        cursorBackgroundSpriteRenderer = cursorBackground.GetComponent<SpriteRenderer>();

        moveCursorAction = moveCursorActionReference.action;

        cursorPosition = Vector2.zero;
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
            float halfCameraHeight = cursorCamera.orthographicSize;
            float halfCameraWidth = halfCameraHeight * cursorCamera.aspect;

            horizontalCameraBounds = new Vector2(
                cursorCamera.transform.position.x - halfCameraWidth,
                cursorCamera.transform.position.x + halfCameraWidth);
            verticalCameraBounds = new Vector2(
                cursorCamera.transform.position.y - halfCameraHeight,
                cursorCamera.transform.position.y + halfCameraHeight);
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

            cursorPosition = ClampToScreen(cursorPosition + cursorDelta);
        }
        else if (cameraSizeChanged)
        {
            cursorPosition = ClampToScreen(cursorPosition);
        }

        cursorWorldPosition = (Vector2)Camera.main.transform.position + cursorPosition;

        if (LockToGrid)
        {
            Vector2 roundedCursorWorldPosition = new Vector2(
                RoundToGrid(cursorWorldPosition.x),
                RoundToGrid(cursorWorldPosition.y));

            transform.position =
                roundedCursorWorldPosition - (Vector2)Camera.main.transform.position;
        }
        else
        {
            transform.position = cursorPosition;
        }

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

    public void UpdateUsingItem(ItemStack item)
    {
        Sprite = item.ItemDefinition.Sprite;
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

    private Vector3 ClampToScreen(Vector3 input)
    {
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

    private void HideIfUnpaused()
    {
        if (!pauseController.GamePaused)
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowIfUnpaused()
    {
        if (!pauseController.GamePaused)
        {
            gameObject.SetActive(true);
        }
    }

    private void PauseController_OnGamePausedSet()
    {
        if (pauseController.GamePaused)
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
        if (pauseController.GamePaused)
        {
            return;
        }

        gameObject.SetActive(!isAttacking);
    }

    public Vector2 GetWorldPosition() => cursorWorldPosition;

    // Round to the nearest number ending in .25 or .75
    private static float RoundToGrid(float value) =>
        Mathf.Round(2f * value - 0.5f) / 2f + 0.25f;
}
