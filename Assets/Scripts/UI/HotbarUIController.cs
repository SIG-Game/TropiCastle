using UnityEngine;
using UnityEngine.UI;

public class HotbarUIController : MonoBehaviour
{
    [SerializeField] private Transform hotbarItemSlotContainer;

    public int HotbarItemIndex { get; private set; }

    private void Awake()
    {
        HotbarItemIndex = 0;
        SelectHotbarItem(HotbarItemIndex);
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            return;
        }

        if (Input.mouseScrollDelta.y != 0f)
        {
            int newHotbarItemIndex = HotbarItemIndex - (int)Mathf.Sign(Input.mouseScrollDelta.y);

            if (newHotbarItemIndex == 10)
                newHotbarItemIndex = 0;
            else if (newHotbarItemIndex == -1)
                newHotbarItemIndex = 9;

            SelectHotbarItem(newHotbarItemIndex);
        }

        ProcessNumberKeys();
    }

    public void SelectHotbarItem(int newHotbarItemIndex)
    {
        hotbarItemSlotContainer.GetChild(HotbarItemIndex).GetComponent<Image>().color = new Color32(173, 173, 173, 255);
        HotbarItemIndex = newHotbarItemIndex;
        hotbarItemSlotContainer.GetChild(HotbarItemIndex).GetComponent<Image>().color = new Color32(140, 140, 140, 255);
    }

    private void ProcessNumberKeys()
    {
        int newHotbarItemIndex = HotbarItemIndex;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            newHotbarItemIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            newHotbarItemIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            newHotbarItemIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            newHotbarItemIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            newHotbarItemIndex = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            newHotbarItemIndex = 5;
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            newHotbarItemIndex = 6;
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            newHotbarItemIndex = 7;
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            newHotbarItemIndex = 8;
        else if (Input.GetKeyDown(KeyCode.Alpha0))
            newHotbarItemIndex = 9;

        if (newHotbarItemIndex != HotbarItemIndex)
            SelectHotbarItem(newHotbarItemIndex);
    }
}
