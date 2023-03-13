using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FishingMinigame : MonoBehaviour
{
    [SerializeField] private GameObject fishingUI;
    [SerializeField] private GameObject fish;
    [SerializeField] private Image fishImage;
    [SerializeField] private PlayerController player;
    [SerializeField] private float fishMinX;
    [SerializeField] private float fishMaxX;
    [SerializeField] private float fishStartPositionMinAbsX;
    [SerializeField] private float fishStartPositionMaxAbsX;
    [SerializeField] private float minCatchFishX;
    [SerializeField] private float maxCatchFishX;
    [SerializeField] private bool logFishInfo;

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

        if (InputManager.Instance.GetLeftClickDownIfUnusedThisFrame())
        {
            PlayerController.ActionDisablingUIOpen = false;
            AttemptToCatchFish();
            fishingUI.SetActive(false);
            return;
        }

        UpdateFishPosition();
    }

    private void OnDestroy()
    {
        Addressables.Release(fishScriptableObjectsLoadHandle);

        player.OnFishingRodUsed -= StartFishingMinigame;
    }

    private void AttemptToCatchFish()
    {
        bool canCatchFish = fish.transform.localPosition.x >= minCatchFishX &&
            fish.transform.localPosition.x <= maxCatchFishX;
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

        SetFishPositionAndDirection();

        SelectRandomFish();

        fishingUI.SetActive(true);
    }

    private void SetFishPositionAndDirection()
    {
        fish.GetComponent<RectTransform>().anchoredPosition = new Vector3(GetRandomFishXPosition(), 0f, 0f);

        if (logFishInfo)
        {
            Debug.Log("Set fish UI position to: " + fish.transform.localPosition.x);
        }

        float randomFishXDirection = Random.Range(0, 2) == 0 ? 1f : -1f;
        fishImage.transform.localScale = new Vector3(randomFishXDirection,
            fishImage.transform.localScale.y, fishImage.transform.localScale.z);
    }

    private void SelectRandomFish()
    {
        int selectedFishIndex = Random.Range(0, fishScriptableObjects.Count);
        selectedFish = fishScriptableObjects[selectedFishIndex];

        if (logFishInfo)
        {
            Debug.Log("Selected fish: " + selectedFish.species);
        }
    }

    private float GetRandomFishXPosition()
    {
        float xPosition = Random.Range(fishStartPositionMinAbsX, fishStartPositionMaxAbsX);

        if (Random.Range(0, 2) == 0)
        {
            xPosition *= -1f;
        }

        return xPosition;
    }

    private void UpdateFishPosition()
    {
        float fishXDirection = fishImage.transform.localScale.x;
        float fishXSpeed = selectedFish.speed * Time.deltaTime;
        fish.transform.localPosition += Vector3.right * fishXDirection * fishXSpeed;

        bool fishMovedPastXPositionLimit = fish.transform.localPosition.x >= fishMaxX ||
            fish.transform.localPosition.x <= fishMinX;
        if (fishMovedPastXPositionLimit)
        {
            ClampFishXPositionToLimit();
            ChangeFishDirection();
        }
    }

    private void ClampFishXPositionToLimit()
    {
        float clampedFishXPosition = Mathf.Clamp(fish.transform.localPosition.x, fishMinX, fishMaxX);
        fish.transform.localPosition = new Vector3(clampedFishXPosition,
                fish.transform.localPosition.y, fish.transform.localPosition.z);
    }

    private void ChangeFishDirection()
    {
        fishImage.transform.localScale = new Vector3(-fishImage.transform.localScale.x,
            fishImage.transform.localScale.y, fishImage.transform.localScale.z);
    }
}
