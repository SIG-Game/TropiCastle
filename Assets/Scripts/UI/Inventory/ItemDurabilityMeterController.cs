using UnityEngine;

public class ItemDurabilityMeterController : MonoBehaviour
{
    [SerializeField] private GameObject durabilityMeterBackground;
    [SerializeField] private RectTransform durabilityMeterFill;

    public void UpdateUsingItem(ItemStack item)
    {
        if (item.TryGetDurabilityProperties(out int durability, out int initialDurability))
        {
            durabilityMeterBackground.SetActive(true);

            durabilityMeterFill.localScale = new Vector3((float)durability / initialDurability,
                durabilityMeterFill.localScale.y, durabilityMeterFill.localScale.z);
        }
        else if (durabilityMeterBackground.activeSelf)
        {
            HideMeter();
        }
    }

    public void HideMeter()
    {
        durabilityMeterBackground.SetActive(false);
    }
}
