using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float sprintSpeedMultiplier;

    Rigidbody2D rb2d;
    CapsuleCollider2D hitbox;
    Direction lastDirection;

    enum Direction { UP, DOWN, LEFT, RIGHT };

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<CapsuleCollider2D>();
        
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
                hitbox.transform.Translate((float) -0.4, 0, 0);
                Debug.Log("Left");
            }
            else if (horizontalInput > 0)
            {
                lastDirection = Direction.RIGHT;
                hitbox.transform.Translate((float) 0.4, 0, 0);
                Debug.Log("Right");
            }
            else if (verticalInput < 0)
            {
                lastDirection = Direction.DOWN;
                hitbox.transform.Translate(0,(float)-0.4, 0);
                Debug.Log("Down");
            } 
            else if (verticalInput > 0) 
            {
                lastDirection = Direction.UP;
                hitbox.transform.Translate(0, (float) 0.4, 0);
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


    void Attack() {

    }
}
