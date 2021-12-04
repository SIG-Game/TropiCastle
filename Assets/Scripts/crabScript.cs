using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crabScript : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //reduces health of player when this is called
    //doesnt let health go below 0
    public void takeDamage(int damage)
    {
        if (currentHealth - damage >= 0)
        {
            currentHealth -= damage;
        }
        else
        {
            currentHealth = 0;
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
}
