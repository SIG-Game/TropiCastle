using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    public float sprintSpeedMultiplier;
    public Sprite front, back, left, right;

    private PlayerController playerController;
    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private Vector2 velocity;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        velocity = Vector2.zero;
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

        velocity = newVelocity;

        if (inputVector != Vector2.zero)
        {
            if (inputVector.x < 0)
            {
                playerController.lastDirection = PlayerController.Direction.LEFT;
                spriteRenderer.sprite = left;
            }
            else if (inputVector.x > 0)
            {
                playerController.lastDirection = PlayerController.Direction.RIGHT;
                spriteRenderer.sprite = right;
            }
            else if (inputVector.y < 0)
            {
                playerController.lastDirection = PlayerController.Direction.DOWN;
                spriteRenderer.sprite = front;
            }
            else
            {
                playerController.lastDirection = PlayerController.Direction.UP;
                spriteRenderer.sprite = back;
            }
        }
    }

    private void FixedUpdate()
    {
        rb2d.MovePosition(transform.position + (Vector3)velocity);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Water"))
        {
            movementSpeed *= 0.5f;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Water"))
        {
            movementSpeed *= 2f;
        }
    }
}
