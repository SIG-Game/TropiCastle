using TMPro;
using UnityEngine;

public class MoneyTextController : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private MoneyController moneyController;

    private void Awake()
    {
        moneyController.OnMoneySet += MoneyController_OnMoneySet;
    }

    private void OnDestroy()
    {
        moneyController.OnMoneySet -= MoneyController_OnMoneySet;
    }

    private void MoneyController_OnMoneySet()
    {
        moneyText.text = moneyController.Money.ToString();
    }
}
