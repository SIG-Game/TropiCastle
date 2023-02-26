using UnityEngine;

public class InventoryFullUIController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryFullUI;
    [SerializeField] private float activeTimeSeconds;

    private float activeTimer;

    public static InventoryFullUIController Instance;

    private void Awake()
    {
        Instance = this;

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

    private void OnDestroy()
    {
        Instance = null;
    }

    public void ShowInventoryFullText()
    {
        inventoryFullUI.SetActive(true);
        activeTimer = 0f;
    }
}
