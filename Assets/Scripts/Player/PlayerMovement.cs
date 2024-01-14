using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private InputActionReference sprintActionReference;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float sprintSpeedMultiplier;
    [SerializeField] private float waterSpeedMultiplier;

    private PlayerController playerController;
    private new Rigidbody2D rigidbody2D;
    private Vector2 velocity;
    private InputAction moveAction;
    private InputAction sprintAction;
    private bool inWater;
    private int waterLayer;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rigidbody2D = GetComponent<Rigidbody2D>();

        velocity = Vector2.zero;

        moveAction = moveActionReference.action;
        sprintAction = sprintActionReference.action;

        inWater = false;

        waterLayer = LayerMask.NameToLayer("Water");
    }

    private void Update()
    {
        if (!playerController.CanMove())
        {
            velocity = Vector2.zero;
            return;
        }

        Vector2 movementInput = moveAction.ReadValue<Vector2>();

        Vector2 newVelocity = movementSpeed * movementInput;

        if (sprintAction.IsPressed())
        {
            newVelocity *= sprintSpeedMultiplier;
        }

        if (inWater)
        {
            newVelocity *= waterSpeedMultiplier;
        }

        velocity = newVelocity;

        if (movementInput != Vector2.zero)
        {
            // If the magnitude of the input's y component is greater than the
            // magnitude of the input's x component, then use a vertical direction
            if (Mathf.Abs(movementInput.y) > Mathf.Abs(movementInput.x))
            {
                playerController.Direction = movementInput.y > 0f ?
                    CharacterDirection.Up : CharacterDirection.Down;
            }
            else
            {
                playerController.Direction = movementInput.x > 0f ?
                    CharacterDirection.Right : CharacterDirection.Left;
            }
        }
    }

    private void FixedUpdate()
    {
        rigidbody2D.MovePosition(transform.position + (Vector3)velocity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == waterLayer)
        {
            inWater = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == waterLayer)
        {
            inWater = false;
        }
    }
}
