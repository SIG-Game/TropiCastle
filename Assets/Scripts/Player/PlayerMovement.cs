using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float sprintSpeedMultiplier;
    [SerializeField] private float waterSpeedMultiplier;

    private PlayerController playerController;
    private Rigidbody2D rb2d;
    private Vector2 velocity;
    private InputAction moveAction;
    private InputAction sprintAction;
    private bool inWater;
    private int waterLayer;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb2d = GetComponent<Rigidbody2D>();

        velocity = Vector2.zero;

        inWater = false;

        waterLayer = LayerMask.NameToLayer("Water");
    }

    private void Start()
    {
        moveAction = InputManager.Instance.GetAction("Move");
        sprintAction = InputManager.Instance.GetAction("Sprint");
    }

    private void Update()
    {
        if (!playerController.CanMove())
        {
            velocity = Vector2.zero;
            return;
        }

        Vector2 inputVector = moveAction.ReadValue<Vector2>();

        Vector2 newVelocity = movementSpeed * inputVector;

        if (sprintAction.IsPressed())
        {
            newVelocity *= sprintSpeedMultiplier;
        }

        if (inWater)
        {
            newVelocity *= waterSpeedMultiplier;
        }

        velocity = newVelocity;

        if (inputVector != Vector2.zero)
        {
            // If the magnitude of the input's y component is greater than the magnitude of the
            // input's x component, then set the sprite based only on the input's y component
            if (Mathf.Abs(inputVector.y) > Mathf.Abs(inputVector.x))
            {
                if (inputVector.y > 0f)
                {
                    playerController.LastDirection = CharacterDirection.Up;
                }
                else
                {
                    playerController.LastDirection = CharacterDirection.Down;
                }
            }
            else
            {
                if (inputVector.x > 0f)
                {
                    playerController.LastDirection = CharacterDirection.Right;
                }
                else
                {
                    playerController.LastDirection = CharacterDirection.Left;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        rb2d.MovePosition(transform.position + (Vector3)velocity);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == waterLayer)
        {
            inWater = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == waterLayer)
        {
            inWater = false;
        }
    }
}
