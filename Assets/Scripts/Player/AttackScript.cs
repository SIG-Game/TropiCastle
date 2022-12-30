using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public int damage;
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            other.GetComponent<enemyScript>().takeDamage(damage); //velocity
            Vector2 difference = other.transform.position - transform.position;
            difference.Normalize();
            other.GetComponent<Rigidbody2D>().AddForce(difference, ForceMode2D.Impulse);
            Debug.Log("Attacking enemy");
        }
    }
}
