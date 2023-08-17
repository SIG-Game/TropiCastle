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
    [SerializeField] private PlayerActionDisablingUIManager playerActionDisablingUIManager;
    [SerializeField] private InventoryFullUIController inventoryFullUIController;
    [SerializeField] private Vector2 catchFishXPositionRange;
    [SerializeField] private bool logSelectedFish;

    public event Action OnFishingStopped = delegate { };
    public event Action OnFishingUIOpened = delegate { };
    public event Action OnFishingUIClosed = delegate { };

    private Animator animator;
    private FishScriptableObject selectedFish;
    private ItemWithAmount selectedFishItem;
    private List<float> fishProbabilityWeights;
    private float fishProbabilityWeightSum;
    private bool catchFailedAnimationStarted;

    private AsyncOperationHandle<IList<FishScriptableObject>> fishScriptableObjectsLoadHandle;
    private IList<FishScriptableObject> fishScriptableObjects;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        fishScriptableObjectsLoadHandle = Addressables.LoadAssetsAsync<FishScriptableObject>("fish", null);
        fishScriptableObjects = fishScriptableObjectsLoadHandle.WaitForCompletion();

        fishProbabilityWeights = new List<float>();
        fishProbabilityWeightSum = 0f;

        foreach (var fishScriptableObject in fishScriptableObjects)
        {
            fishProbabilityWeights.Add(fishScriptableObject.probabilityWeight);
            fishProbabilityWeightSum += fishScriptableObject.probabilityWeight;
        }
    }

    private void Update()
    {
        if (!fishingUI.activeSelf || PauseController.Instance.GamePaused)
        {
            return;
        }

        // Get both inputs so that neither can be used elsewhere
        bool useItemButtonInput = InputManager.Instance.GetUseItemButtonDownIfUnusedThisFrame();
        bool fishButtonInput = InputManager.Instance.GetFishButtonDownIfUnusedThisFrame();

        if ((useItemButtonInput || fishButtonInput) && !catchFailedAnimationStarted)
        {
            AttemptToCatchFish();
        }
    }

    private void OnDestroy()
    {
        Addressables.Release(fishScriptableObjectsLoadHandle);

        OnFishingStopped = delegate { };
        OnFishingUIOpened = delegate { };
        OnFishingUIClosed = delegate { };
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
        playerItemInWorld.ShowCharacterItemInWorld(selectedFishItem);

        HideFishingUI();

        void afterCatchDialogueAction()
        {
            playerInventory.AddItem(selectedFishItem);
            playerItemInWorld.HideCharacterItemInWorld();
        }

        DialogueBox.Instance.PlayDialogue($"You caught a {selectedFish.species.ToLowerInvariant()}!\n" +
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

        playerActionDisablingUIManager.ActionDisablingUIOpen = true;

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

        selectedFish = fishScriptableObjects[selectedFishIndex];
        fishUI.Speed = selectedFish.speed;
        fishUI.SetColor(selectedFish.fishUIColor);

        selectedFishItem = new ItemWithAmount(
            ItemScriptableObject.FromName(selectedFish.name), 1);

        if (logSelectedFish)
        {
            Debug.Log("Selected fish: " + selectedFish.species);
        }
    }

    private void HideFishingUI()
    {
        fishingUI.SetActive(false);
        fishUI.gameObject.SetActive(false);

        playerActionDisablingUIManager.ActionDisablingUIOpen = false;

        OnFishingUIClosed();
    }
}
