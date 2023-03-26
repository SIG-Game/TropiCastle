﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

public class FishingMinigame : MonoBehaviour
{
    [SerializeField] private GameObject fishingUI;
    [SerializeField] private FishUIController fishUI;
    [SerializeField] private PlayerController player;
    [SerializeField] private float minCatchFishX;
    [SerializeField] private float maxCatchFishX;
    [SerializeField] private bool logSelectedFish;

    private FishScriptableObject selectedFish;
    private Inventory playerInventory;

    private AsyncOperationHandle<IList<FishScriptableObject>> fishScriptableObjectsLoadHandle;
    private IList<FishScriptableObject> fishScriptableObjects;

    private void Awake()
    {
        fishScriptableObjectsLoadHandle = Addressables.LoadAssetsAsync<FishScriptableObject>("fish", null);
        fishScriptableObjects = fishScriptableObjectsLoadHandle.WaitForCompletion();

        player.OnFishingRodUsed += StartFishingMinigame;
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

        if (leftClickInput || fishButtonInput)
        {
            PlayerController.ActionDisablingUIOpen = false;
            AttemptToCatchFish();
            fishingUI.SetActive(false);
            fishUI.gameObject.SetActive(false);
            return;
        }
    }

    private void OnDestroy()
    {
        Addressables.Release(fishScriptableObjectsLoadHandle);

        player.OnFishingRodUsed -= StartFishingMinigame;
    }

    private void AttemptToCatchFish()
    {
        bool canCatchFish = fishUI.transform.localPosition.x >= minCatchFishX &&
            fishUI.transform.localPosition.x <= maxCatchFishX;
        if (canCatchFish)
        {
            CatchFish();
        }
    }

    private void CatchFish()
    {
        ItemScriptableObject caughtFishItem = Resources.Load<ItemScriptableObject>("Items/" + selectedFish.name);
        Action afterCatchDialogueAction = () => playerInventory.AddItem(caughtFishItem, 1);
        DialogueBox.Instance.PlayDialogue($"You caught a {selectedFish.species.ToLowerInvariant()}!\n" +
            $"{selectedFish.description}", afterCatchDialogueAction);
    }

    private void StartFishingMinigame()
    {
        if (playerInventory.IsFull())
        {
            InventoryFullUIController.Instance.ShowInventoryFullText();
            return;
        }

        if (fishingUI.activeSelf)
        {
            Debug.LogWarning($"{nameof(StartFishingMinigame)} started with {nameof(fishingUI)} active");
            return;
        }

        PlayerController.ActionDisablingUIOpen = true;

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
}
