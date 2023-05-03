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
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerItemInWorldController playerItemInWorld;
    [SerializeField] private Vector2 catchFishXPositionRange;
    [SerializeField] private bool logSelectedFish;

    private Animator animator;
    private FishScriptableObject selectedFish;
    private Inventory playerInventory;
    private bool catchFailedAnimationStarted;

    private AsyncOperationHandle<IList<FishScriptableObject>> fishScriptableObjectsLoadHandle;
    private IList<FishScriptableObject> fishScriptableObjects;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        fishScriptableObjectsLoadHandle = Addressables.LoadAssetsAsync<FishScriptableObject>("fish", null);
        fishScriptableObjects = fishScriptableObjectsLoadHandle.WaitForCompletion();

        player.OnFishingRodUsed += StartFishing;
    }

    private void Start()
    {
        playerInventory = player.GetInventory();
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

        player.OnFishingRodUsed -= StartFishing;
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
    }

    private void CatchFish()
    {
        ItemScriptableObject caughtFishItem = Resources.Load<ItemScriptableObject>("Items/" + selectedFish.name);

        playerItemInWorld.ShowPlayerItemInWorld(caughtFishItem.sprite);

        void afterCatchDialogueAction()
        {
            playerInventory.AddItem(caughtFishItem, 1);
            playerItemInWorld.HidePlayerItemInWorld();
        }

        DialogueBox.Instance.PlayDialogue($"You caught a {selectedFish.species.ToLowerInvariant()}!\n" +
            $"{selectedFish.description}", afterCatchDialogueAction);

        HideFishingUI();
    }

    private void StartFishing()
    {
        if (playerInventory.IsFull())
        {
            InventoryFullUIController.Instance.ShowInventoryFullText();
            return;
        }

        if (fishingUI.activeSelf)
        {
            Debug.LogWarning($"{nameof(StartFishing)} called with {nameof(fishingUI)} active");
            return;
        }

        PlayerController.ActionDisablingUIOpen = true;

        catchFailedAnimationStarted = false;

        fishUI.ResetFishUIImage();
        fishUI.SetFishUIPositionAndDirection();

        SelectRandomFish();

        fishingUI.SetActive(true);
        fishUI.gameObject.SetActive(true);
    }

    private void SelectRandomFish()
    {
        int selectedFishIndex = Random.Range(0, fishScriptableObjects.Count);
        selectedFish = fishScriptableObjects[selectedFishIndex];
        fishUI.Speed = selectedFish.speed;
        fishUI.SetColor(selectedFish.fishUIColor);

        if (logSelectedFish)
        {
            Debug.Log("Selected fish: " + selectedFish.species);
        }
    }

    private void HideFishingUI()
    {
        fishingUI.SetActive(false);
        fishUI.gameObject.SetActive(false);

        PlayerController.ActionDisablingUIOpen = false;
    }
}
