using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    [SerializeField] private GameObject fishingUI;
    [SerializeField] private GameObject fish;
    [SerializeField] private Image fishImage;
    [SerializeField] private PlayerController player;

    [HideInInspector] public bool canCatch = false;

    private Direction direction = Direction.left;
    private FishScriptableObject[] fishScriptableObjects;
    private FishScriptableObject selectedFish;

    private void Awake()
    {
        fishScriptableObjects = Resources.LoadAll<FishScriptableObject>("Fish");
    }

    private void Update()
    {
        if (fishingUI.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) && canCatch)
            {
                DialogueBox.Instance.PlayDialogue(new List<string> { selectedFish.species + "\n" + selectedFish.description });
                ItemScriptableObject caughtFishItem = Resources.Load<ItemScriptableObject>("Items/" + selectedFish.name);
                player.GetInventory().AddItem(caughtFishItem, 1);
                EndFishing();
            }

            fish.transform.localPosition += new Vector3(0.1f * selectedFish.speed * (int)direction, 0f, 0f);

            if (fish.transform.localPosition.x >= 200 || fish.transform.localPosition.x <= -200)
            {
                ChangeDirection();
            }
        }
    }

    private void ChangeDirection()
    {
        direction = (Direction)(-(int)direction);
        fishImage.transform.localScale = new Vector3(-fishImage.transform.localScale.x,
            fishImage.transform.localScale.y, fishImage.transform.localScale.z);
    }

    private void EndFishing()
    {
        fishingUI.SetActive(false);
    }

    public void StartFishing()
    {
        if (!fishingUI.activeSelf)
        {
            if (Random.Range(0, 2) == 0)
            {
                fishImage.transform.localScale = new Vector3(-1f, fishImage.transform.localScale.y, fishImage.transform.localScale.z);
                direction = Direction.right;
            }
            else
            {
                fishImage.transform.localScale = new Vector3(1f, fishImage.transform.localScale.y, fishImage.transform.localScale.z);
                direction = Direction.left;
            }

            int selectedFishIndex = Random.Range(0, fishScriptableObjects.Length);
            selectedFish = fishScriptableObjects[selectedFishIndex];

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

public enum Direction
{
    left = -1,
    right = 1
}
