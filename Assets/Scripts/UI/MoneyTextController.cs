using TMPro;
using UnityEngine;

public class MoneyTextController : MonoBehaviour
{
    [SerializeField] private MoneyController moneyController;
    
    private TMP_Text moneyText;

    private void Awake()
    {
        moneyText = GetComponent<TMP_Text>();

        moneyController.OnMoneySet += MoneyController_OnMoneySet;
    }

    private void OnDestroy()
    {
        moneyController.OnMoneySet -= MoneyController_OnMoneySet;
    }

    private void MoneyController_OnMoneySet()
    {
        moneyText.text = $"Money: {moneyController.Money}";
    }
}
