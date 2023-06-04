using UnityEngine;

public class ItemDurabilityMeterController : MonoBehaviour
{
    [SerializeField] private GameObject durabilityMeterBackground;
    [SerializeField] private RectTransform durabilityMeterFill;

    public void UpdateUsingItem(ItemWithAmount item)
    {
        if (item.instanceProperties is BreakableItemInstanceProperties
            breakableItemInstanceProperties &&
            item.itemData is BreakableItemScriptableObject
            breakableItemScriptableObject)
        {
            UpdateDurabilityMeter(
                breakableItemInstanceProperties.Durability,
                breakableItemScriptableObject.InitialDurability);
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
