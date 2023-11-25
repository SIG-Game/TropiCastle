using UnityEngine;

public class ItemInteractableDependencies : MonoBehaviour
{
    [SerializeField] private ChestUIController chestUIController;
    [SerializeField] private CampfireUIController campfireUIController;

    public ChestUIController ChestUIController => chestUIController;

    public CampfireUIController CampfireUIController => campfireUIController;
}
