using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FishingUIController : MonoBehaviour
{
    [SerializeField] private GameObject fishingUI;
    [SerializeField] private FishUIController fishUI;
    [SerializeField] private Transform hookTransform;
    [SerializeField] private PlayerActionDisablingUIManager playerActionDisablingUIManager;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private CharacterItemInWorldController playerItemInWorld;
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private InventoryFullUIController inventoryFullUIController;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Vector2 catchFishXPositionRange;

    public event Action OnFishingStopped = () => {};
    public event Action OnFishingUIOpened = () => {};
    public event Action OnFishingUIClosed = () => {};

    private Animator animator;
    private WeightedRandomSelector fishSelector;
    private ItemScriptableObject selectedFishDefinition;
    private ItemStack selectedFishItem;
    private bool catchFailedAnimationStarted;

    private IList<ItemScriptableObject> fishItemScriptableObjects;
    private AsyncOperationHandle<IList<ItemScriptableObject>> itemsLoadHandle;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        itemsLoadHandle =
            Addressables.LoadAssetsAsync<ItemScriptableObject>("item", null);

        fishItemScriptableObjects = itemsLoadHandle.WaitForCompletion()
            .Where(x => x.HasProperty("FishSpeed")).ToList();

        List<float> fishWeights = fishItemScriptableObjects
            .Select(x => x.GetFloatProperty("FishProbabilityWeight")).ToList();

        fishSelector = new WeightedRandomSelector(fishWeights);
    }

    private void Update()
    {
        if (!fishingUI.activeSelf || PauseController.Instance.GamePaused)
        {
            return;
        }

        if (inputManager.GetUseItemButtonDownIfUnusedThisFrame() &&
            !catchFailedAnimationStarted)
        {
            AttemptToCatchFish();
        }
    }

    private void OnDestroy()
    {
        if (itemsLoadHandle.IsValid())
        {
            Addressables.Release(itemsLoadHandle);
        }
    }

    private void AttemptToCatchFish()
    {
        float catchFishXMin = hookTransform.position.x + catchFishXPositionRange.x;
        float catchFishXMax = hookTransform.position.x + catchFishXPositionRange.y;

        bool canCatchFish = fishUI.transform.position.x >= catchFishXMin &&
            fishUI.transform.position.x <= catchFishXMax;
        if (canCatchFish)
        {
            CatchFish();
        }
        else
        {
            animator.SetTrigger("Catch Failed");
            catchFailedAnimationStarted = true;
        }

        OnFishingStopped();
    }

    private void CatchFish()
    {
        playerItemInWorld.ShowItem(selectedFishItem);

        HideFishingUI();

        void afterCatchDialogueAction()
        {
            playerInventory.AddItem(selectedFishItem);
            playerItemInWorld.Hide();
        }

        dialogueBox.PlayDialogue(
            $"You caught a {selectedFishDefinition.Name.ToLowerInvariant()}!\n" +
                selectedFishDefinition.GetStringProperty("FishDescription"),
            afterCatchDialogueAction);
    }

    public void StartFishing()
    {
        if (fishingUI.activeSelf)
        {
            Debug.LogWarning($"{nameof(StartFishing)} called with {nameof(fishingUI)} active");
            return;
        }

        selectedFishDefinition = fishItemScriptableObjects[fishSelector.SelectIndex()];
        fishUI.Speed = selectedFishDefinition.GetFloatProperty("FishSpeed");

        selectedFishItem = new ItemStack(selectedFishDefinition, 1);

        if (!playerInventory.CanAddItem(selectedFishItem))
        {
            inventoryFullUIController.ShowInventoryFullText();
            return;
        }

        catchFailedAnimationStarted = false;

        fishingUI.SetActive(true);
        fishUI.gameObject.SetActive(true);

        playerActionDisablingUIManager.ActionDisablingUIOpen = true;

        OnFishingUIOpened();
    }

    private void HideFishingUI()
    {
        fishingUI.SetActive(false);
        fishUI.gameObject.SetActive(false);

        playerActionDisablingUIManager.ActionDisablingUIOpen = false;

        OnFishingUIClosed();
    }
}
