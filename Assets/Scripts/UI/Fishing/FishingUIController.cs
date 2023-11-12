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
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private CharacterItemInWorldController playerItemInWorld;
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private InventoryFullUIController inventoryFullUIController;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Vector2 catchFishXPositionRange;
    [SerializeField] private bool logSelectedFish;

    public event Action OnFishingStopped = () => {};
    public event Action OnFishingUIOpened = () => {};
    public event Action OnFishingUIClosed = () => {};

    private Animator animator;
    private WeightedRandomSelector fishSelector;
    private FishItemScriptableObject selectedFish;
    private ItemStack selectedFishItem;
    private bool catchFailedAnimationStarted;

    private IList<FishItemScriptableObject> fishItemScriptableObjects;
    private AsyncOperationHandle<IList<FishItemScriptableObject>> fishItemsLoadHandle;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        fishItemsLoadHandle = Addressables
            .LoadAssetsAsync<FishItemScriptableObject>("item", null);

        fishItemScriptableObjects = fishItemsLoadHandle.WaitForCompletion();

        List<float> fishWeights = fishItemScriptableObjects
            .Select(x => x.probabilityWeight).ToList();

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
        if (fishItemsLoadHandle.IsValid())
        {
            Addressables.Release(fishItemsLoadHandle);
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
            $"You caught a {selectedFish.name.ToLowerInvariant()}!\n" +
            $"{selectedFish.description}", afterCatchDialogueAction);
    }

    public void StartFishing()
    {
        if (fishingUI.activeSelf)
        {
            Debug.LogWarning($"{nameof(StartFishing)} called with {nameof(fishingUI)} active");
            return;
        }

        selectedFish = fishItemScriptableObjects[fishSelector.SelectIndex()];
        fishUI.Speed = selectedFish.speed;

        selectedFishItem = new ItemStack(selectedFish, 1);

        if (logSelectedFish)
        {
            Debug.Log("Selected fish: " + selectedFish.name);
        }

        if (!playerInventory.CanAddItem(selectedFishItem))
        {
            inventoryFullUIController.ShowInventoryFullText();
            return;
        }

        catchFailedAnimationStarted = false;

        fishingUI.SetActive(true);
        fishUI.gameObject.SetActive(true);

        OnFishingUIOpened();
    }

    private void HideFishingUI()
    {
        fishingUI.SetActive(false);
        fishUI.gameObject.SetActive(false);

        OnFishingUIClosed();
    }
}
