using UnityEngine;

public class HookController : MonoBehaviour
{
    [SerializeField] private FishingMinigame fishingMinigame;

    private void OnTriggerEnter2D(Collider2D fish)
    {
        fishingMinigame.canCatch = true;
    }

    private void OnTriggerExit2D(Collider2D fish)
    {
        fishingMinigame.canCatch = false;
    }
}
