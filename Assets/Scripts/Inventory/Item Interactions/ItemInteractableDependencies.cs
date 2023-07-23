using UnityEngine;

public class ItemInteractableDependencies : MonoBehaviour
{
    [SerializeField] private ChestUIController chestUIController;

    public ChestUIController GetChestUIController() => chestUIController;
}
