using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float sprintSpeedMultiplier;
    public int maxHealth = 100;
    public int currentHealth;
    public bool isAttacking = false;

    public Sprite front, back, left, right;
    public SpriteRenderer spriteRender;
    public Animator animator;

    public Rigidbody2D rb2d;
    private Direction lastDirection = Direction.DOWN;

    public enum Direction { UP, DOWN, LEFT, RIGHT };

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        bool moving = (horizontalInput != 0 || verticalInput != 0) ? true : false;

        if (moving && !isAttacking) {
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

        if (isAttacking)
        {
            rb2d.velocity = Vector2.zero;
        }

        /*
        //test takeDamage
        if(Input.GetKeyDown(KeyCode.Space))
        {
            takeDamage(20);
        }
        
        //test getHealth
        if(Input.GetKeyDown(KeyCode.M))
        {
            addHealth(20);
        }
        */
    }

    //reduces health of player when this is called
    //doesnt let health go below 0
    public void takeDamage(int damage)
    {
        if (currentHealth - damage >= 0)  
        {
            currentHealth -= damage;
        } else
        {
            currentHealth = 0;
        }
    }

    public void Attack() {
        if (Input.GetButtonDown("Fire1") && !isAttacking) {
            switch (lastDirection) {
                case Direction.UP:
                    animator.Play("Swing Up");
                    break;

                case Direction.DOWN:
                    animator.Play("Swing Down");
                    break;

                case Direction.LEFT:
                    animator.Play("Swing Left");
                    break;

                case Direction.RIGHT:
                    animator.Play("Swing Right");
                    break;
            }
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
    
    //increases health of a player when called
    //doesnt let health go above 100
    public void addHealth(int health)
    {
        if(currentHealth + health <= maxHealth)
        {
            currentHealth += health;
        } else
        {
            currentHealth = 100;
        }
    }

    /*
    void OnCollisionEnter2D (Collision2D col)
    {
        if(col.gameObject.tag.Equals("Enemy"))
        {
            takeDamage(10);
        }
    }
    */
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("Enemy"))
        {
            takeDamage(10);
        }
    }
    
}

