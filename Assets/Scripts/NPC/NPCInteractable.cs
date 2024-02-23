using System.Collections;
using UnityEngine;

public abstract class NPCInteractable : MonoBehaviour, IInteractable
{
    private const float returnToDefaultDirectionWaitTime = 1.25f;

    private CharacterDirectionController directionController;
    private Coroutine waitThenReturnToDefaultDirectionCoroutine;
    private WaitForSeconds returnToDefaultDirectionWaitForSeconds;

    protected virtual void Awake()
    {
        directionController = GetComponent<CharacterDirectionController>();

        returnToDefaultDirectionWaitForSeconds =
            new WaitForSeconds(returnToDefaultDirectionWaitTime);
    }

    protected void FacePlayer(PlayerController player) =>
        directionController.UseOppositeOfDirection(player.Direction);

    protected void StartWaitThenReturnToDefaultDirectionCouroutine()
    {
        if (waitThenReturnToDefaultDirectionCoroutine != null)
        {
            StopCoroutine(waitThenReturnToDefaultDirectionCoroutine);
        }

        waitThenReturnToDefaultDirectionCoroutine =
            StartCoroutine(WaitThenReturnToDefaultDirectionCoroutine());
    }

    private IEnumerator WaitThenReturnToDefaultDirectionCoroutine()
    {
        yield return returnToDefaultDirectionWaitForSeconds;

        directionController.UseDefaultDirection();

        waitThenReturnToDefaultDirectionCoroutine = null;
    }

    public abstract void Interact();
}
