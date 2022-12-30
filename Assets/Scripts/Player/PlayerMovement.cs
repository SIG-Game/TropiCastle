using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    public float sprintSpeedMultiplier;
    public Sprite front, back, left, right;

    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!playerController.CanMove())
        {
            playerController.SetVelocity(Vector2.zero);
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

        playerController.SetVelocity(newVelocity);

        if (inputVector != Vector2.zero)
        {
            if (inputVector.x < 0)
            {
                playerController.SetLastDirection(PlayerController.Direction.LEFT);
                spriteRenderer.sprite = left;
            }
            else if (inputVector.x > 0)
            {
                playerController.SetLastDirection(PlayerController.Direction.RIGHT);
                spriteRenderer.sprite = right;
            }
            else if (inputVector.y < 0)
            {
                playerController.SetLastDirection(PlayerController.Direction.DOWN);
                spriteRenderer.sprite = front;
            }
            else
            {
                playerController.SetLastDirection(PlayerController.Direction.UP);
                spriteRenderer.sprite = back;
            }
        }
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
