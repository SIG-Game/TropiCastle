using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float sprintSpeedMultiplier;
    [SerializeField] private float waterSpeedMultiplier;

    private PlayerController playerController;
    private Rigidbody2D rb2d;
    private Vector2 velocity;
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

    private void Update()
    {
        if (!playerController.CanMove())
        {
            velocity = Vector2.zero;
            return;
        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 inputVector = new Vector2(horizontalInput, verticalInput);

        if (inputVector.sqrMagnitude > 1f)
        {
            inputVector.Normalize();
        }

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
                playerController.LastDirection = PlayerDirection.Left;
            }
            else if (inputVector.x > 0)
            {
                playerController.LastDirection = PlayerDirection.Right;
            }
            else if (inputVector.y < 0)
            {
                playerController.LastDirection = PlayerDirection.Down;
            }
            else
            {
                playerController.LastDirection = PlayerDirection.Up;
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
