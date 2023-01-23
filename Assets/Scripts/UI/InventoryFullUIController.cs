using UnityEngine;

public class InventoryFullUIController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryFullUI;
    [SerializeField] private float activeTimeSeconds;

    private float activeTimer;

    private void Awake()
    {
        activeTimer = 0f;
    }

    private void Update()
    {
        if (activeTimer < activeTimeSeconds)
        {
            activeTimer += Time.deltaTime;

            if (activeTimer >= activeTimeSeconds)
            {
                inventoryFullUI.SetActive(false);
            }
        }
    }

    public void ShowInventoryFullText()
    {
        inventoryFullUI.SetActive(true);
        activeTimer = 0f;
    }
}
