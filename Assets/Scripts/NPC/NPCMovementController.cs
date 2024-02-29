using System.Collections.Generic;
using UnityEngine;

public class NPCMovementController : MonoBehaviour
{
    [SerializeField] private List<Vector2> targetPositions;
    [SerializeField] private float movementSpeed;

    private CharacterDirectionController directionController;
    private new Rigidbody2D rigidbody2D;
    private Vector2 targetPosition;
    private int currentTargetPositionIndex;

    private void Awake()
    {
        directionController = GetComponent<CharacterDirectionController>();
        rigidbody2D = GetComponent<Rigidbody2D>();

        targetPosition = targetPositions[currentTargetPositionIndex];
    }

    private void FixedUpdate()
    {
        Vector2 nextPosition = Vector2.MoveTowards(
            rigidbody2D.position, targetPosition, movementSpeed);

        Vector2 velocity = nextPosition - rigidbody2D.position;

        if (Mathf.Abs(velocity.y) > Mathf.Abs(velocity.x))
        {
            directionController.Direction = velocity.y > 0f ?
                CharacterDirection.Up : CharacterDirection.Down;
        }
        else
        {
            directionController.Direction = velocity.x > 0f ?
                CharacterDirection.Right : CharacterDirection.Left;
        }

        rigidbody2D.MovePosition(nextPosition);

        if (nextPosition == targetPosition)
        {
            currentTargetPositionIndex++;
            if (currentTargetPositionIndex == targetPositions.Count)
            {
                currentTargetPositionIndex = 0;
            }

            targetPosition = targetPositions[currentTargetPositionIndex];
        }
    }
}
