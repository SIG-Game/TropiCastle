using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float sprintSpeedMultiplier;
    public int maxHealth = 100;
    public int currentHealth;

    public Sprite front, back, left, right;
    public SpriteRenderer spriteRender;


    public Rigidbody2D rb2d;
    private Direction lastDirection;

    public enum Direction { UP, DOWN, LEFT, RIGHT };

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
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
                spriteRender.sprite = left;
                //Debug.Log("Left");
            }
            else if (horizontalInput > 0)
            {
                lastDirection = Direction.RIGHT;
                spriteRender.sprite = right;
                //Debug.Log("Right");
            }
            else if (verticalInput < 0)
            {
                lastDirection = Direction.DOWN;
                spriteRender.sprite = front;
                //Debug.Log("Down");
            } 
            else if (verticalInput > 0) 
            {
                lastDirection = Direction.UP;
                spriteRender.sprite = back;
                //Debug.Log("Up");
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
        
        /* test takeDamage
        if(Input.GetKeyDown(KeyCode.Space))
        {
            takeDamage(20);
        }
        */
    }

    //reduces health of player when this is called
    public void takeDamage(int damage)
    {
        if (currentHealth - damage >= 0) //doesnt let health go below 0 
        {
            currentHealth -= damage;
        } else
        {
            currentHealth = 0;
        }
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

