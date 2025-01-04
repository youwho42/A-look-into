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
    public bool setAmountOverTime;
    public int CurrentAmount { get { return currentAmount; } }
    public void SetupUI(int max, int stack, Vector3 position)
    {
        maxAmount = max;
        currentAmount = stack;
        amountText.text = $"{stack}/{max}";

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

    public void SetAmount(int _maxAmount, int stackAmount)
    {
        maxAmount = _maxAmount;
        currentAmount = stackAmount;
        
        amountText.text = $"{currentAmount}/{maxAmount}";
    }
    public void ChangeAmount(int amount)
    {
        currentAmount += amount;
        if (currentAmount > maxAmount)
            currentAmount = 0;
        else if (currentAmount < 0)
            currentAmount = maxAmount;
        amountText.text = $"{currentAmount}/{maxAmount}";
    }
    public void HalfAmount()
    {
        int a = (int)Mathf.Ceil(currentAmount * 0.5f);
        currentAmount = a;
        if (currentAmount < 1)
            currentAmount = 1;
        amountText.text = $"{currentAmount}/{maxAmount}";
    }
    public void DoubleAmount()
    {
        currentAmount *= 2;
        if (currentAmount > maxAmount)
            currentAmount = maxAmount;
        
        amountText.text = $"{currentAmount}/{maxAmount}";
    }
    public void StartSetAmount(int amount)
    {
        setAmountOverTime = true;
        StartCoroutine(SetAmountCo(amount));
    }
    public void StopSetAmount()
    {
        setAmountOverTime = false;
        StopAllCoroutines();
    }
    IEnumerator SetAmountCo(int amount)
    {
        ChangeAmount(amount);
        yield return new WaitForSeconds(0.4f);
        float timer = Time.time + 2;
        while (setAmountOverTime)
        {
            ChangeAmount(amount);
            float t = Time.time > timer ? 0.03f : 0.15f;
            yield return new WaitForSeconds(t);
        }
        yield return null;
    }
}
