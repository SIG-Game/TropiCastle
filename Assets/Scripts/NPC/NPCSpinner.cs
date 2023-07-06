using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpinner : MonoBehaviour
{
    [SerializeField] private CharacterDirectionController directionController;
    [SerializeField] private Vector2Int spinsBeforeWaitRange;
    [SerializeField] private Vector2 timeBetweenSpinsSecondsRange;

    private List<CharacterDirection> spinDirections;
    private Coroutine spinCoroutine;

    private void Awake()
    {
        spinDirections = new List<CharacterDirection> { CharacterDirection.Down,
            CharacterDirection.Left, CharacterDirection.Up, CharacterDirection.Right };
    }

    public void StartSpinning()
    {
        spinCoroutine = StartCoroutine(SpinCoroutine());
    }

    public void StopSpinning()
    {
        if (spinCoroutine != null)
        {
            StopCoroutine(spinCoroutine);
        }
    }

    private IEnumerator SpinCoroutine()
    {
        while (true)
        {
            int numberOfSpinsBeforeWait = Random.Range(spinsBeforeWaitRange.x,
                spinsBeforeWaitRange.y + 1);

            for (int i = 0; i < numberOfSpinsBeforeWait; ++i)
            {
                foreach (CharacterDirection direction in spinDirections)
                {
                    directionController.Direction = direction;
                    yield return new WaitForSeconds(0.175f);
                }
            }

            directionController.Direction = CharacterDirection.Down;

            float waitBeforeSpinningSeconds = Random.Range(timeBetweenSpinsSecondsRange.x,
                timeBetweenSpinsSecondsRange.y);

            yield return new WaitForSeconds(waitBeforeSpinningSeconds);
        }
    }
}
