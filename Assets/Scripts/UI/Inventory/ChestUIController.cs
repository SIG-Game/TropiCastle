using UnityEngine;
using UnityEngine.InputSystem;

// TODO: Better integrate this class with InventoryUIController
public class ChestUIController : MonoBehaviour
{
    [SerializeField] private GameObject chestUI;
    [SerializeField] private ItemSlotContainerController chestItemSlotContainerController;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private InputActionReference inventoryActionReference;

    private InputAction inventoryAction;

    public static ChestUIController Instance;
    public static bool ChestUIOpen;

    static ChestUIController()
    {
        ChestUIOpen = false;
    }

    private void Awake()
    {
        Instance = this;

        inventoryAction = inventoryActionReference.action;
    }

    // Must run after the InventoryUIController Update method so that the inventory
    // input press for closing the chest UI is not reused to open the inventory UI
    private void Update()
    {
        bool closeChestInputPressed = inventoryAction.WasPressedThisFrame() ||
            Input.GetKeyDown(KeyCode.Escape) ||
            InputManager.Instance.GetInteractButtonDownIfUnusedThisFrame();

        bool closeChestUI = closeChestInputPressed
            && chestUI.activeInHierarchy;
        if (closeChestUI)
        {
            ChestUIOpen = false;
            PauseController.Instance.GamePaused = false;

            chestUI.SetActive(false);

            inventoryUIController.InvokeOnInventoryUIClosedEvent();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                InputManager.Instance.EscapeKeyUsedThisFrame = true;
            }
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void ShowChestUI()
    {
        ChestUIOpen = true;
        PauseController.Instance.GamePaused = true;

        chestUI.SetActive(true);
    }

    public void SetChestInventory(Inventory chestInventory)
    {
        chestItemSlotContainerController.SetInventory(chestInventory);
    }
}
