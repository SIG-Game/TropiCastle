using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    public GameObject canvas;
    public GameObject Hook;
    public GameObject fish;
    public Image image;
    public Sprite fishImageL;
    public Sprite fishImageR;
    public ScriptableFish[] data;
    int chosen = 0;
    public Direction _direction = Direction.left;
    public bool canCatch = false;
    public PlayerController player;

    private ItemScriptableObject fishScriptableObject;

    void Start()
    {
        fishScriptableObject = Resources.Load<ItemScriptableObject>("Items/Fish");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canCatch)
        {
            
            DialogueBox.Instance.PlayDialogue(new List<string> { data[chosen].species + "\n" + data[chosen].description });
            Debug.Log("Fish Caught" + chosen + " " + data[chosen].species);
            player.GetInventory().AddItem(fishScriptableObject, 1);
            endFishing();
        }
        fish.transform.localPosition = fish.transform.localPosition + new Vector3(0.1f * data[chosen].speed * (int)_direction, 0, 0);
        if (fish.transform.localPosition.x >= 200 || fish.transform.localPosition.x <= -200)
        {
            // Debug.Log(transform.localPosition.x);
            ChangeDirection();
        }
    }
    void ChangeDirection()
    {
        switch (_direction)
        {
            case Direction.right:
                image.sprite = fishImageL;
                _direction = Direction.left;
                break;
            case Direction.left:
                image.sprite = fishImageR;
                _direction = Direction.right;
                break;
        }
    }

    public void startFishing()
    {
        if (!canvas.activeSelf)
        {
            if (Random.Range(-1, 1) == -1)
            {
                image.sprite = fishImageR;
                _direction = Direction.right;
            } else {
                image.sprite = fishImageL;
                _direction = Direction.left;
            }
            chosen = Random.Range(0, data.Length);
            Debug.Log("SELECTED: " + chosen + " " + data[chosen].species + " " + data[chosen].speed);
            canvas.SetActive(true);
            int positionX = Random.Range(80, 190);
            if (Random.Range(-1, 1) == -1)
            {
                positionX = positionX * -1;
            }
            fish.GetComponent<RectTransform>().anchoredPosition = new Vector3(positionX, 0, 0);
            Debug.Log("Spawned at: " + fish.transform.localPosition.x);
        }
    }
    public void endFishing()
    {
        canvas.SetActive(false);
    }
}

public enum Direction
{
    left = -1,
    right = 1
}
