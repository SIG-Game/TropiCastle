using UnityEngine;

public class ItemDurabilityMeterController : MonoBehaviour
{
    [SerializeField] private GameObject durabilityMeterBackground;
    [SerializeField] private RectTransform durabilityMeterFill;

    public void UpdateUsingItem(ItemStack item)
    {
        if (item.TryGetDurabilityProperties(out int durability, out int initialDurability))
        {
            UpdateDurabilityMeter(durability, initialDurability);
        }
        else if (MeterIsActive())
        {
            HideMeter();
        }
    }

    public void HideMeter()
    {
        durabilityMeterBackground.SetActive(false);
    }

    private void UpdateDurabilityMeter(float durability, float maxDurability)
    {
        durabilityMeterBackground.SetActive(true);

        float durabilityMeterFillXScale = durability / maxDurability;

        durabilityMeterFill.localScale = new Vector3(durabilityMeterFillXScale,
            durabilityMeterFill.localScale.y, durabilityMeterFill.localScale.z);
    }

    private bool MeterIsActive() => durabilityMeterBackground.activeSelf;
}
