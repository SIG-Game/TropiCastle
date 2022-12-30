using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour
{
    private enum State
    {
        Chilling,
        Chasing,
        Knockbacked,
    }
    private State state;
    public int maxHealth = 100;
    public int currentHealth;

    public List<ItemWithAmount> DroppedLoot;

    public Transform player;
    public EnemySpawner spawner;
    public float speed = 5f;

    // TODO: This could be moved to a singleton
    public GameObject itemWorldPrefab;

    private Vector2 movement;

    private Vector3 playerColliderOffset;
    Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        playerColliderOffset = player.GetComponent<BoxCollider2D>().offset;
        currentHealth = maxHealth;
        state = State.Chilling;
        //StartCoroutine("Wait");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.transform.position + playerColliderOffset - transform.position;
        direction.Normalize();
        movement = direction;
    }
    
    private void FixedUpdate()
    {
        if(state == State.Chilling)
        {
            float targetRange = 2.5f;
            if(Vector3.Distance(transform.position, player.position) < targetRange)
            {
                StartCoroutine("Wait");
                state = State.Chasing;
            }
        }
        if(state == State.Chasing) {
            moveEnemy(movement);
        }
    }
   

    //reduces health of player when this is called
    //doesnt let health go below 0
    public void takeDamage(int damage)
    {
        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
        }
        else
        {
            currentHealth = 0;
            spawner.SpawnedEnemyDied();

            // TODO: This doesn't work properly for loot items with amount value > 1
            // This should be revisited if stackable items get added
            foreach (ItemWithAmount loot in DroppedLoot)
            {
                ItemWorld.DropItem(itemWorldPrefab, transform.position, loot);
            }
            Destroy(gameObject);
        }
    }

    //increases health of a player when called
    //doesnt let health go above 100
    public void addHealth(int health)
    {
        if (currentHealth + health <= maxHealth)
        {
            currentHealth += health;
        }
        else
        {
            currentHealth = 100;
        }
    }

    //simple move towards player
    void moveEnemy(Vector2 direction)
    {
        //use the velocity
        rb2d.MovePosition((Vector2)transform.position + (direction * speed * Time.deltaTime));
    }

    //gets knockback when in contact with player
    //can update to when getting hit by weapon by changing the tag
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            state = State.Knockbacked;
            Vector2 difference = transform.position - other.transform.position;
            difference.Normalize();
            //transform.position = new Vector2(transform.position.x + difference.x, transform.position.y + difference.y);
            transform.GetComponent<Rigidbody2D>().AddForce(difference * 10, ForceMode2D.Force);
            StopCoroutine("Wait");
            StartCoroutine("Wait");
            //state = State.Chasing;
        }
    }
   
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.5f);
        state = State.Chasing;
        transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
    
}
