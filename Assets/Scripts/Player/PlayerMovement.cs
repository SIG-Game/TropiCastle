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
    private bool inWater;
    private int waterLayer;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb2d = GetComponent<Rigidbody2D>();

        velocity = Vector2.zero;

        moveAction = GetComponent<PlayerInput>().currentActionMap["Move"];

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

        Vector2 inputVector = moveAction.ReadValue<Vector2>();

        Vector2 newVelocity = movementSpeed * inputVector;

        if (Input.GetButton("Sprint"))
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
            if (inputVector.x < 0)
            {
                playerController.LastDirection = CharacterDirection.Left;
            }
            else if (inputVector.x > 0)
            {
                playerController.LastDirection = CharacterDirection.Right;
            }
            else if (inputVector.y < 0)
            {
                playerController.LastDirection = CharacterDirection.Down;
            }
            else
            {
                playerController.LastDirection = CharacterDirection.Up;
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
