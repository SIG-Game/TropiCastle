using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{

    public GameObject player;
    public Vector2 playerPos;
    public PlayerController.Direction lastDirection;

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
            transform.position = new Vector2(playerPos.x, playerPos.y + (float) 0.2);
        } else if (lastDirection == PlayerController.Direction.DOWN) {
            transform.position = new Vector2(playerPos.x, playerPos.y - (float) 0.2);
        } else if (lastDirection == PlayerController.Direction.LEFT) {
            transform.position = new Vector2(playerPos.x - (float) 0.2, playerPos.y);
        } else if (lastDirection == PlayerController.Direction.RIGHT) {
            transform.position = new Vector2(playerPos.x + (float) 0.2, playerPos.y);
        }
    }
}
