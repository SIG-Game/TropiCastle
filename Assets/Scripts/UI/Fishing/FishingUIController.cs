using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

public class FishingUIController : MonoBehaviour
{
    [SerializeField] private GameObject fishingUI;
    [SerializeField] private FishUIController fishUI;
    [SerializeField] private Transform hookTransform;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private CharacterItemInWorldController playerItemInWorld;
    [SerializeField] private InventoryFullUIController inventoryFullUIController;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Vector2 catchFishXPositionRange;
    [SerializeField] private bool logSelectedFish;

    public event Action OnFishingStopped = () => {};
    public event Action OnFishingUIOpened = () => {};
    public event Action OnFishingUIClosed = () => {};

    private Animator animator;
    private FishItemScriptableObject selectedFish;
    private ItemStack selectedFishItem;
    private List<float> fishProbabilityWeights;
    private float fishProbabilityWeightSum;
    private bool catchFailedAnimationStarted;

    private IList<FishItemScriptableObject> fishItemScriptableObjects;
    private AsyncOperationHandle<IList<FishItemScriptableObject>> fishItemsLoadHandle;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        fishItemsLoadHandle = Addressables
            .LoadAssetsAsync<FishItemScriptableObject>("item", null);

        fishItemScriptableObjects = fishItemsLoadHandle.WaitForCompletion();

        fishProbabilityWeights = new List<float>();
        fishProbabilityWeightSum = 0f;

        foreach (var fishItemScriptableObject in fishItemScriptableObjects)
        {
            fishProbabilityWeights.Add(fishItemScriptableObject.probabilityWeight);
            fishProbabilityWeightSum += fishItemScriptableObject.probabilityWeight;
        }
    }

    private void Update()
    {
        if (!fishingUI.activeSelf || PauseController.Instance.GamePaused)
        {
            return;
        }

        // Get both inputs so that neither can be used elsewhere
        bool useItemButtonInput =
            inputManager.GetUseItemButtonDownIfUnusedThisFrame();
        bool fishButtonInput =
            inputManager.GetFishButtonDownIfUnusedThisFrame();

        if ((useItemButtonInput || fishButtonInput) && !catchFailedAnimationStarted)
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

        DialogueBox.Instance.PlayDialogue(
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

        SelectRandomFish();

        if (!playerInventory.CanAddItem(selectedFishItem))
        {
            inventoryFullUIController.ShowInventoryFullText();
            return;
        }

        catchFailedAnimationStarted = false;

        fishUI.ResetFishUIImage();
        fishUI.SetRandomXPosition();
        fishUI.SetRandomXDirection();
        fishUI.UpdateXVelocity();

        fishingUI.SetActive(true);
        fishUI.gameObject.SetActive(true);

        OnFishingUIOpened();
    }

    private void SelectRandomFish()
    {
        int selectedFishIndex = -1;
        float fishSelector = Random.Range(0f, fishProbabilityWeightSum);
        float fishSelectionLowerBound = 0f;

        for (int i = 0; i < fishProbabilityWeights.Count; ++i)
        {
            if (fishSelector >= fishSelectionLowerBound &&
                fishSelector <= fishSelectionLowerBound + fishProbabilityWeights[i])
            {
                selectedFishIndex = i;
                break;
            }

            fishSelectionLowerBound += fishProbabilityWeights[i];
        }

        selectedFish = fishItemScriptableObjects[selectedFishIndex];
        fishUI.Speed = selectedFish.speed;

        selectedFishItem = new ItemStack(selectedFish, 1);

        if (logSelectedFish)
        {
            Debug.Log("Selected fish: " + selectedFish.name);
        }
    }

    private void HideFishingUI()
    {
        fishingUI.SetActive(false);
        fishUI.gameObject.SetActive(false);

        OnFishingUIClosed();
    }
}
