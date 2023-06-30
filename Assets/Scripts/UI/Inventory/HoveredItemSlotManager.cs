using UnityEngine;

public class HoveredItemSlotManager : MonoBehaviour
{
    public int HoveredItemIndex { get; set; }
    public Inventory HoveredInventory { get; set; }

    private void Awake()
    {
        HoveredItemIndex = -1;
    }
}
