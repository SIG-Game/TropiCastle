using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
    public GameObject canvas;
    public GameObject Hook;
    public GameObject fish;
    public ScriptableFish[] data;

    int choosen = 0;
    public Direction _direction = Direction.left;
    public bool canCatch = false;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && canCatch) {
            DialogueBox.Instance.PlayDialogue(new List<string>{data[choosen].species + "\n" + data[choosen].description});
            Debug.Log("Fish Caught" + choosen + " " + data[choosen].species);
            endFishing();
        }
        fish.transform.position = fish.transform.position + new Vector3(0.1f * data[choosen].speed * (int)_direction,0,0);
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
                _direction = Direction.left;
                break;
            case Direction.left:
                _direction = Direction.right;
                break;
        }
    }

    public void startFishing()
    {
        if (!canvas.activeSelf)
        {
            if (Random.Range(-1,1) == -1) {
                _direction = Direction.right;
            }
            choosen = Random.Range(0, data.Length);
            Debug.Log("SELECTED: " + choosen + " " + data[choosen].species + " " + data[choosen].speed);
            canvas.SetActive(true);
            fish.GetComponent<RectTransform>().anchoredPosition = new Vector3(Random.Range(-190, 190), 0, 0);
        }
    }
    public void endFishing()
    {
        canvas.SetActive(false);
    }
}
