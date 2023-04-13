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
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerFishItemInWorldController playerFishItemInWorld;
    [SerializeField] private float minCatchFishX;
    [SerializeField] private float maxCatchFishX;
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
        bool leftClickInput = InputManager.Instance.GetLeftClickDownIfUnusedThisFrame();
        bool fishButtonInput = InputManager.Instance.GetFishButtonDownIfUnusedThisFrame();

        if ((leftClickInput || fishButtonInput) && !catchFailedAnimationStarted)
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
        bool canCatchFish = fishUI.transform.localPosition.x >= minCatchFishX &&
            fishUI.transform.localPosition.x <= maxCatchFishX;
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

        playerFishItemInWorld.ShowPlayerFishItemInWorld(caughtFishItem.sprite);

        Action afterCatchDialogueAction = () =>
        {
            playerInventory.AddItem(caughtFishItem, 1);
            playerFishItemInWorld.HidePlayerFishItemInWorld();
        };

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
