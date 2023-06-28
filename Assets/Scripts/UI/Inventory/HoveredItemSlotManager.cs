using UnityEngine;

public class HoveredItemSlotManager : MonoBehaviour
{
    public int HoveredItemIndex { get; set; }

    private void Awake()
    {
        HoveredItemIndex = -1;
    }
}
