using UnityEngine;

public class SellerNPC : NPCInteractable
{
    [SerializeField] private NPCProductScriptableObject product;

    [Inject] private InventoryUIManager inventoryUIManager;
    [Inject] private PlayerController playerController;
    [Inject] private SellerNPCUIController sellerNPCUIController;

    protected override void Awake()
    {
        base.Awake();

        this.InjectDependencies();
    }

    public override void Interact()
    {
        FacePlayer(playerController);

        sellerNPCUIController.DisplayProduct(product);

        inventoryUIManager.OnInventoryUIClosed +=
            InventoryUIManager_OnInventoryUIClosed;
    }

    private void InventoryUIManager_OnInventoryUIClosed()
    {
        directionController.UseDefaultDirection();

        inventoryUIManager.OnInventoryUIClosed -=
            InventoryUIManager_OnInventoryUIClosed;
    }
}
