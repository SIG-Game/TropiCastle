using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyScript : MonoBehaviour
{
    public Transform Player;
    public GameObject bullet;

    //private float shotCooldown;
    public float startShotCD;
    // Start is called before the first frame update
    void Start()
    {
        //shotCooldown = startShotCD;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = new Vector2(Player.position.x - transform.position.x, Player.position.y - transform.position.y);
        transform.up = direction;

        float targetRange = 6f;
        if (Vector3.Distance(transform.position, Player.position) < targetRange)
        {
            if(Time.time > startShotCD)
            {
                Instantiate(bullet, transform.position, transform.rotation);
                float fireRate = 2f;
                startShotCD = Time.time + fireRate;
            }
        }

        /*
        if(shotCooldown <= 0)
        {
            Instantiate(bullet, transform.position, transform.rotation);
            shotCooldown = startShotCD;
        } else
        {
            shotCooldown -= Time.deltaTime;
        }
        */
    }
}
