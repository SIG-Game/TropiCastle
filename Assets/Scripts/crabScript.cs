using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crabScript : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public GameObject itemWorld;
    // public Item.ItemType droppedItemType;
    public List<Item> DroppedLoot;

    //private Transform target;
    public float speed;


    Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        //target = FindObjectOfType<PlayerController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        
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
            foreach (Item loot in DroppedLoot) {
                ItemWorld.DropItem(transform.position, loot.itemType, loot.amount);
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

    //gets knockback when in contact with player
    //can update to when getting hit by weapon by changing the tag
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Vector2 difference = transform.position - other.transform.position;
            transform.position = new Vector2(transform.position.x + difference.x, transform.position.y + difference.y);
        }
    }
    */
    //testing basic enemy ai
    //doesn't work yet

    /*
    public void FollowPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }
    */
}
