using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{

    public GameObject player;
    public Vector2 playerPos;
    public GameObject enemy = null;
    public PlayerController.Direction lastDirection;
    public bool enemyInHitbox;

    // Start is called before the first frame update
    void Start()
    {
        lastDirection = player.GetComponent<PlayerController>().getLastDir();
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = player.transform.position;
        lastDirection = player.GetComponent<PlayerController>().getLastDir();
        if (lastDirection == PlayerController.Direction.UP) {
            transform.position = new Vector2(playerPos.x,playerPos.y + (float) 0.2);
        } else if (lastDirection == PlayerController.Direction.DOWN) {
            transform.position = new Vector2(playerPos.x,playerPos.y - (float) 0.2);
        } else if (lastDirection == PlayerController.Direction.LEFT) {
            transform.position = new Vector2(playerPos.x - (float) 0.2, playerPos.y);
        } else if (lastDirection == PlayerController.Direction.RIGHT) {
            transform.position = new Vector2(playerPos.x + (float) 0.2, playerPos.y);
        }
        Attack();
    }

    void Attack() {
        if (Input.GetButtonDown("Fire1") && enemy != null) {
            // do damage to enemy
            enemy.GetComponent<crabScript>().takeDamage(10);
            Debug.Log("Attacking enemy");
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            enemy = other.gameObject;
            enemyInHitbox = true;
            Debug.Log("Enemy entered hitbox");
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            enemyInHitbox = false;
            enemy = null;
            Debug.Log("Enemy left hitbox");
        }
    }

}
