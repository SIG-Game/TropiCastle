using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    private int health = 100;

    public void takeDamage(int damage)
    {
        if (health - damage > 0)
        {
            health -= damage;
        }
        else
        {
            health = 0;
        }
    }
}
