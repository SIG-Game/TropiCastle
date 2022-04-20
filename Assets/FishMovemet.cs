using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public int Speed = 1;
    Direction _direction = Direction.left;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = transform.position + new Vector3(0.1f * Speed * (int)_direction,0,0);
        if (transform.localPosition.x >= 200 || transform.localPosition.x <= -200)
        {
            Debug.Log(transform.localPosition.x);
            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        switch (_direction)
        {
            case Direction.right:
                _direction = Direction.left;
                break;
            case Direction.left:
                _direction = Direction.right;
                break;
        }
    }
}

public enum Direction
{
    left = -1,
    right = 1
}