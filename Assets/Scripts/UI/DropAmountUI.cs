using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropAmountUI : MonoBehaviour
{
    public Button okButton;
    int maxAmount;
    int currentAmount;
    public TextMeshProUGUI amountText;

    public int CurrentAmount { get { return currentAmount; } }
    public void SetupUI(int max)
    {
        maxAmount = max;
        currentAmount = max;
        amountText.text = max.ToString();
    }
    public void ResetUI()
    {
        maxAmount = 0;
        currentAmount = 0;
    }

    public void SetAmount(int amount)
    {
        currentAmount += amount;
        if (currentAmount > maxAmount)
            currentAmount = 0;
        else if (currentAmount < 0)
            currentAmount = maxAmount;
        amountText.text = currentAmount.ToString();
    }
}
