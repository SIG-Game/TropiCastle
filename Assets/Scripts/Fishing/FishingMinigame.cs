﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    [SerializeField] private GameObject fishingUI;
    [SerializeField] private GameObject fish;
    [SerializeField] private Image fishImage;
    [SerializeField] private PlayerController player;

    [HideInInspector] public bool canCatch = false;

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

            fish.transform.localPosition += new Vector3(selectedFish.speed * Time.deltaTime * -fishImage.transform.localScale.x, 0f, 0f);

            if (fish.transform.localPosition.x >= 200f || fish.transform.localPosition.x <= -200f)
            {
                fish.transform.localPosition = new Vector3(Mathf.Clamp(fish.transform.localPosition.x, -200f, 200f),
                    fish.transform.localPosition.y, fish.transform.localPosition.z);

                ChangeDirection();
            }
        }
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

    public void StartFishing()
    {
        if (!fishingUI.activeSelf)
        {
            fishImage.transform.localScale = new Vector3(Random.Range(0, 2) == 0 ? 1f : -1f,
                fishImage.transform.localScale.y, fishImage.transform.localScale.z);

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
