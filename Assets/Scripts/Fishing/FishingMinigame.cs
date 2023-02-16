﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    [SerializeField] private GameObject fishingUI;
    [SerializeField] private GameObject fish;
    [SerializeField] private Image fishImage;
    [SerializeField] private PlayerController player;
    [SerializeField] private float minCatchFishX;
    [SerializeField] private float maxCatchFishX;
    [SerializeField] private bool logFishInfo;

    private FishScriptableObject selectedFish;
    private Inventory playerInventory;

    private AsyncOperationHandle<IList<FishScriptableObject>> fishScriptableObjectsLoadHandle;

    private void Awake()
    {
        fishScriptableObjectsLoadHandle = Addressables.LoadAssetsAsync<FishScriptableObject>("fish", null);

        player.OnFishingRodUsed += Player_OnFishingRodUsed;
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
            AttemptToCatchFish();
            fishingUI.SetActive(false);
            return;
        }

        float fishXDirection = fishImage.transform.localScale.x;
        float fishXSpeed = selectedFish.speed * Time.deltaTime;
        fish.transform.localPosition += Vector3.right * fishXDirection * fishXSpeed;

        ChangeFishDirectionIfFishAtEdge();
    }

    private void OnDestroy()
    {
        Addressables.Release(fishScriptableObjectsLoadHandle);

        player.OnFishingRodUsed -= Player_OnFishingRodUsed;
    }

    private void AttemptToCatchFish()
    {
        bool canCatchFish = fish.transform.localPosition.x >= minCatchFishX && fish.transform.localPosition.x <= maxCatchFishX;
        if (canCatchFish)
        {
            CatchFish();
        }
    }

    private void CatchFish()
    {
        DialogueBox.Instance.PlayDialogue(selectedFish.species + "\n" + selectedFish.description);
        ItemScriptableObject caughtFishItem = Resources.Load<ItemScriptableObject>("Items/" + selectedFish.name);
        playerInventory.AddItem(caughtFishItem, 1);
    }

    private IEnumerator StartFishingCoroutine()
    {
        if (playerInventory.IsFull())
        {
            DialogueBox.Instance.PlayDialogue("You cannot fish because your inventory is full.");
            yield break;
        }

        while (!fishScriptableObjectsLoadHandle.IsDone)
        {
            yield return fishScriptableObjectsLoadHandle;
        }

        if (fishingUI.activeSelf)
        {
            Debug.LogWarning($"{nameof(StartFishingCoroutine)} started with {nameof(fishingUI)} active");
            yield break;
        }

        float randomFishXDirection = Random.Range(0, 2) == 0 ? 1f : -1f;
        fishImage.transform.localScale = new Vector3(randomFishXDirection,
            fishImage.transform.localScale.y, fishImage.transform.localScale.z);

        fish.GetComponent<RectTransform>().anchoredPosition = new Vector3(GetRandomFishXPosition(), 0f, 0f);

        if (logFishInfo)
        {
            Debug.Log("Set fish UI position to: " + fish.transform.localPosition.x);
        }

        SelectRandomFish();

        fishingUI.SetActive(true);
    }

    private void SelectRandomFish()
    {
        int selectedFishIndex = Random.Range(0, fishScriptableObjectsLoadHandle.Result.Count);
        selectedFish = fishScriptableObjectsLoadHandle.Result[selectedFishIndex];

        if (logFishInfo)
        {
            Debug.Log("Selected fish: " + selectedFish.species);
        }
    }

    private float GetRandomFishXPosition()
    {
        float xPosition = Random.Range(80f, 190f);

        if (Random.Range(0, 2) == 0)
        {
            xPosition *= -1f;
        }

        return xPosition;
    }

    private void ChangeFishDirectionIfFishAtEdge()
    {
        if (fish.transform.localPosition.x >= 200f || fish.transform.localPosition.x <= -200f)
        {
            fish.transform.localPosition = new Vector3(Mathf.Clamp(fish.transform.localPosition.x, -200f, 200f),
                fish.transform.localPosition.y, fish.transform.localPosition.z);

            ChangeFishDirection();
        }
    }

    private void ChangeFishDirection()
    {
        fishImage.transform.localScale = new Vector3(-fishImage.transform.localScale.x,
            fishImage.transform.localScale.y, fishImage.transform.localScale.z);
    }

    private void Player_OnFishingRodUsed()
    {
        StartCoroutine(StartFishingCoroutine());
    }
}
