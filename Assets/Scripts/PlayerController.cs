using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float sprintSpeedMultiplier;

    public Rigidbody2D rb2d;
    private Direction lastDirection;

    public enum Direction { UP, DOWN, LEFT, RIGHT };

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        bool moving = (horizontalInput != 0 || verticalInput != 0) ? true : false;

        if (moving) {
            if (horizontalInput < 0)
            {
                lastDirection = Direction.LEFT;
                Debug.Log("Left");
            }
            else if (horizontalInput > 0)
            {
                lastDirection = Direction.RIGHT;
                Debug.Log("Right");
            }
            else if (verticalInput < 0)
            {
                lastDirection = Direction.DOWN;
                Debug.Log("Down");
            } 
            else if (verticalInput > 0) 
            {
                lastDirection = Direction.UP;
                Debug.Log("Up");
            }
        }

        

        Vector2 inputVector = (new Vector2(horizontalInput, verticalInput));
        inputVector.Normalize();

        Vector2 velocity = movementSpeed * inputVector;

        if (Input.GetButton("Sprint"))
        {
            velocity *= sprintSpeedMultiplier;
        }
        Attack();
        rb2d.velocity = velocity;
    }

    public void Attack() {
        if (Input.GetButton("Fire1")) {
            Debug.Log("attacked");
        }
    } 

    // // When enemy enters collider hitbox, set to true
    // void OnTriggerEnter2D(Collider other) {

    // }

    // // When enemy exits, set within range to false
    // void OnTriggerExit2D(Collider other) {

    // }

    public Direction getLastDir() {
        return lastDirection;
    }
}

