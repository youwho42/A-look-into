using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropAmountUI : MonoBehaviour
{
    public Button okButton;
    int maxAmount;
    int currentAmount;
    public TextMeshProUGUI amountText;
    public RectTransform canvasRectTransform;
    public int CurrentAmount { get { return currentAmount; } }
    public void SetupUI(int max, Vector3 position)
    {
        maxAmount = max;
        currentAmount = max;
        amountText.text = max.ToString();

        // Convert the screen point to a position in the canvas
        
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, null, out anchoredPosition);

        // Apply the anchored position to the UI element
        GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        //GetComponent<RectTransform>().anchoredPosition = position;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(okButton.gameObject);
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
