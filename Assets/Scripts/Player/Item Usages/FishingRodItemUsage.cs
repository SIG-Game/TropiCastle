using UnityEngine;

public class FishingRodItemUsage : MonoBehaviour, IItemUsage
{
    public void UseItem(PlayerController playerController)
    {
        playerController.Fish();
    }
}
