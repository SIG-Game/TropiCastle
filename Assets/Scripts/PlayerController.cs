using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float sprintSpeedMultiplier;
    public int maxHealth = 100;
    public int currentHealth;

    Rigidbody2D rb2d;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 inputVector = (new Vector2(horizontalInput, verticalInput));
        inputVector.Normalize();

        Vector2 velocity = movementSpeed * inputVector;

        if (Input.GetButton("Sprint"))
        {
            velocity *= sprintSpeedMultiplier;
        }

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
}
