using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class NPCTransactionUIController : NPCInventoryUIController
{
    [SerializeField] private GameObject transactionUIPrefab;
    [SerializeField] private Transform transactionUIParent;
    [SerializeField] protected MoneyController playerMoneyController;
    [SerializeField] private string buttonText;
    [SerializeField] private bool flipArrow;

    protected abstract int TransactionCount { get; }

    private List<Button> buttons;

    protected override void Awake()
    {
        base.Awake();

        buttons = new List<Button>();
    }

    protected override void DisplayUI()
    {
        buttons.Clear();

        foreach (Transform transactionUI in transactionUIParent)
        {
            Destroy(transactionUI.gameObject);
        }

        for (int i = 0; i < TransactionCount; i++)
        {
            GameObject transactionUI =
                Instantiate(transactionUIPrefab, transactionUIParent);

            var button = transactionUI.transform.GetChild(0).GetComponent<Button>();

            button.onClick.AddListener(new UnityAction(GetButtonOnClickListener(i)));

            button.transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>().text = buttonText;

            buttons.Add(button);

            transactionUI.transform.GetChild(1).GetChild(1)
                .GetComponent<TextMeshProUGUI>().text = GetTransactionMoneyText(i);

            if (flipArrow)
            {
                transactionUI.transform.GetChild(2)
                    .GetComponent<RectTransform>().localScale = new Vector3(-1f, 1f, 1f);
            }

            transactionUI.transform.GetChild(3)
                .GetComponent<TextMeshProUGUI>().text = GetTransactionItemText(i);
        }

        base.DisplayUI();
    }

    protected override void InventoryUIHeldItemController_OnItemHeld() =>
        buttons.ForEach(x => x.interactable = false);

    protected override void InventoryUIHeldItemController_OnHidden() =>
        buttons.ForEach(x => x.interactable = true);

    protected abstract string GetTransactionItemText(int index);

    protected abstract string GetTransactionMoneyText(int index);

    protected abstract Action GetButtonOnClickListener(int index);
}