using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookCatch : MonoBehaviour
{
    // ScriptableFish[] fishInfo = FishingMinigame.GetComponent<ScriptableFish>;
    // public GameObject canvas;
    // public GameObject fish;
    // public ScriptableFish data;
    public FishingMinigame FishingMinigame;
    public PlayerController playerController;

    bool canCatch = false;
    void OnTriggerEnter2D(Collider2D fish)
    {
        // Debug.Log("entered");
        FishingMinigame.canCatch = true;
    }
    void OnTriggerExit2D(Collider2D fish)
    {
        FishingMinigame.canCatch = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && canCatch)
        {
            playerController.GetInventory().AddItem(Item.ItemType.fish, 1);
            // DialogueBox.Instance.PlayDialogue(new List<string> {fish.species + "\n" + fish.description });
            // Debug.Log("Fish Caught");
            // // DisplayInfo();
            FishingMinigame.endFishing();
        }
    }

    void DisplayInfo(ScriptableFish fish)
    {
        DialogueBox.Instance.PlayDialogue(new List<string> {fish.species + "\n" + fish.description });
            Debug.Log("Fish Caught");
    }
}
