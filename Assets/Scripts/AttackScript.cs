using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            other.GetComponent<crabScript>().takeDamage(10);
            Debug.Log("Attacking enemy");
        }
    }
}
