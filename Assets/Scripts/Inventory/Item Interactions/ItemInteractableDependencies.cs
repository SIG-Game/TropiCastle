using UnityEngine;

public class ItemInteractableDependencies : MonoBehaviour
{
    [SerializeField] private ChestUIController chestUIController;
    [SerializeField] private CampfireUIController campfireUIController;

    public ChestUIController GetChestUIController() => chestUIController;

    public CampfireUIController GetCampfireUIController() => campfireUIController;
}
