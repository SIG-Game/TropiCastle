using System.Collections;
using TMPro;
using UnityEngine;

public class InventoryAdditionTextUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inventoryAdditionText;
    [SerializeField] private float waitTimeBeforeDespawnSeconds;

    private WaitForSeconds beforeDespawnWaitForSeconds;

    private void Awake()
    {
        beforeDespawnWaitForSeconds = new WaitForSeconds(waitTimeBeforeDespawnSeconds);

        StartCoroutine(WaitThenDespawn());
    }

    private IEnumerator WaitThenDespawn()
    {
        yield return beforeDespawnWaitForSeconds;

        Destroy(gameObject);
    }

    public void SetText(string text)
    {
        inventoryAdditionText.text = text;
    }
}
