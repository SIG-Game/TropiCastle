using System.Collections.Generic;
using UnityEngine;

public class SellerNPC : NPCInteractable
{
    [SerializeField] private List<NPCProductScriptableObject> products;

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

        sellerNPCUIController.DisplayProducts(products);

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
