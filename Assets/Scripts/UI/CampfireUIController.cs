using System;
using System.Collections.Generic;
using UnityEngine;

public class CampfireUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> campfireUIGameObjects;
    [SerializeField] private InventoryUIController campfireInventoryUIController;
    [SerializeField] private RectTransform progressArrowMask;
    [SerializeField] private InventoryUIManager inventoryUIManager;
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private Vector2 playerInventoryUIPosition;
    [SerializeField] private float maxProgressArrowMaskWidth;

    public event Action OnCampfireUIClosed = () => {};

    public void Show()
    {
        playerInventoryUI.anchoredPosition = playerInventoryUIPosition;

        inventoryUIManager.SetCurrentInventoryUIGameObjects(campfireUIGameObjects);
        inventoryUIManager.EnableCurrentInventoryUI();

        inventoryUIManager.OnInventoryUIClosed +=
            InventoryUIManager_OnCampfireUIClosed;
    }

    public void SetInventory(Inventory campfireInventory)
    {
        campfireInventoryUIController.SetInventory(campfireInventory);
    }

    public void UpdateCookTimeProgressArrow(float cookTimeProgress, float recipeCookTime)
    {
        float progressArrowMaskWidth =
           cookTimeProgress / recipeCookTime * maxProgressArrowMaskWidth;

        progressArrowMask.sizeDelta = new Vector2(progressArrowMaskWidth,
            progressArrowMask.sizeDelta.y);
    }

    public void HideCookTimeProgressArrow()
    {
        progressArrowMask.sizeDelta = new Vector2(0f, progressArrowMask.sizeDelta.y);
    }

    private void InventoryUIManager_OnCampfireUIClosed()
    {
        OnCampfireUIClosed();

        campfireInventoryUIController.UnsetInventory();

        inventoryUIManager.OnInventoryUIClosed -=
            InventoryUIManager_OnCampfireUIClosed;
    }
}
