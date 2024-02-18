using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FishingUIController : MonoBehaviour
{
    [SerializeField] private FishUIController fishUIController;
    [SerializeField] private HookUIController hookUIController;
    [SerializeField] private Transform hookTransform;
    [SerializeField] private CharacterItemInWorldController playerItemInWorld;
    [SerializeField] private Vector2 catchFishXPositionRange;

    [Inject] private DialogueBox dialogueBox;
    [Inject] private InputManager inputManager;
    [Inject] private InventoryFullUIController inventoryFullUIController;
    [Inject] private PauseController pauseController;
    [Inject] private PlayerActionDisablingUIManager playerActionDisablingUIManager;
    [Inject("PlayerInventory")] private Inventory playerInventory;

    public event Action OnFishingStopped = () => {};
    public event Action OnFishingUIOpened = () => {};
    public event Action OnFishingUIClosed = () => {};

    private Animator animator;
    private CanvasGroup canvasGroup;
    private WeightedRandomSelector fishSelector;
    private ItemScriptableObject selectedFishDefinition;
    private ItemStack selectedFishItem;
    private bool catchFailedAnimationStarted;

    private IList<ItemScriptableObject> fishItemScriptableObjects;
    private AsyncOperationHandle<IList<ItemScriptableObject>> itemsLoadHandle;

    private void Awake()
    {
        this.InjectDependencies();

        animator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();

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
        if (canvasGroup.alpha == 0f || pauseController.GamePaused)
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

        bool canCatchFish = fishUIController.transform.position.x >= catchFishXMin &&
            fishUIController.transform.position.x <= catchFishXMax;
        if (canCatchFish)
        {
            CatchFish();
        }
        else
        {
            fishUIController.StopMovement();
            hookUIController.StopMovement();
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
            $"You caught a {selectedFishDefinition.DisplayName.ToLowerInvariant()}!\n" +
                selectedFishDefinition.GetStringProperty("FishDescription"),
            afterCatchDialogueAction);
    }

    public void StartFishing()
    {
        if (canvasGroup.alpha == 1f)
        {
            Debug.LogWarning($"{nameof(StartFishing)} called with fishing UI visible");
            return;
        }

        selectedFishDefinition = fishItemScriptableObjects[fishSelector.SelectIndex()];
        fishUIController.Speed = selectedFishDefinition.GetFloatProperty("FishSpeed");

        selectedFishItem = new ItemStack(selectedFishDefinition, 1);

        if (!playerInventory.CanAddItem(selectedFishItem))
        {
            inventoryFullUIController.ShowInventoryFullText();
            return;
        }

        catchFailedAnimationStarted = false;

        fishUIController.StartMovement();
        hookUIController.StartMovement();

        canvasGroup.alpha = 1f;

        playerActionDisablingUIManager.ActionDisablingUIOpen = true;

        OnFishingUIOpened();
    }

    private void HideFishingUI()
    {
        fishUIController.StopMovement();
        hookUIController.StopMovement();

        canvasGroup.alpha = 0f;

        playerActionDisablingUIManager.ActionDisablingUIOpen = false;

        OnFishingUIClosed();
    }
}
