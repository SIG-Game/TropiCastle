using System.Collections;
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

    private FishScriptableObject selectedFish;

    private AsyncOperationHandle<IList<FishScriptableObject>> fishScriptableObjectsLoadHandle;

    private void Awake()
    {
        fishScriptableObjectsLoadHandle = Addressables.LoadAssetsAsync<FishScriptableObject>("fish", null);
    }

    private void Update()
    {
        if (fishingUI.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) && fish.transform.localPosition.x >= minCatchFishX && fish.transform.localPosition.x <= maxCatchFishX)
            {
                DialogueBox.Instance.PlayDialogue(selectedFish.species + "\n" + selectedFish.description);
                ItemScriptableObject caughtFishItem = Resources.Load<ItemScriptableObject>("Items/" + selectedFish.name);
                player.GetInventory().AddItem(caughtFishItem, 1);
                EndFishing();
            }

            fish.transform.localPosition += new Vector3(selectedFish.speed * Time.deltaTime * fishImage.transform.localScale.x, 0f, 0f);

            if (fish.transform.localPosition.x >= 200f || fish.transform.localPosition.x <= -200f)
            {
                fish.transform.localPosition = new Vector3(Mathf.Clamp(fish.transform.localPosition.x, -200f, 200f),
                    fish.transform.localPosition.y, fish.transform.localPosition.z);

                ChangeDirection();
            }
        }
    }

    private void OnDestroy()
    {
        Addressables.Release(fishScriptableObjectsLoadHandle);
    }

    private void ChangeDirection()
    {
        fishImage.transform.localScale = new Vector3(-fishImage.transform.localScale.x,
            fishImage.transform.localScale.y, fishImage.transform.localScale.z);
    }

    private void EndFishing()
    {
        fishingUI.SetActive(false);
    }

    public IEnumerator StartFishing()
    {
        if (player.GetInventory().IsFull())
        {
            DialogueBox.Instance.PlayDialogue("You cannot fish because your inventory is full.");
            yield break;
        }

        while (!fishScriptableObjectsLoadHandle.IsDone)
        {
            yield return fishScriptableObjectsLoadHandle;
        }

        if (!fishingUI.activeSelf)
        {
            fishImage.transform.localScale = new Vector3(Random.Range(0, 2) == 0 ? 1f : -1f,
                fishImage.transform.localScale.y, fishImage.transform.localScale.z);

            int selectedFishIndex = Random.Range(0, fishScriptableObjectsLoadHandle.Result.Count);
            selectedFish = fishScriptableObjectsLoadHandle.Result[selectedFishIndex];

            Debug.Log("Selected fish: " + selectedFish.species);

            fishingUI.SetActive(true);

            int positionX = Random.Range(80, 190);

            if (Random.Range(0, 2) == 0)
            {
                positionX *= -1;
            }

            fish.GetComponent<RectTransform>().anchoredPosition = new Vector3(positionX, 0, 0);
            Debug.Log("Spawned fish at: " + fish.transform.localPosition.x);
        }
    }
}
