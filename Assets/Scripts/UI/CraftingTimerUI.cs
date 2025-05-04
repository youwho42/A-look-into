using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTimerUI : MonoBehaviour
{
   
    public Image timerImage;
    public Canvas timerCanvas;
    public Color activeColor;
    public Color inactiveColor;

    private void Start()
    {
        HideTimer();
    }
    public void ShowTimer()
    {
        timerCanvas.gameObject.SetActive(true);
    }

    public void HideTimer()
    {
        timerCanvas.gameObject.SetActive(false);
    }
   

    public void SetCraftingTimer(int time, int maxTime)
    {
        timerImage.fillAmount = NumberFunctions.RemapNumber(time-1, 0, maxTime, 0, 1);
    }
    public void SetTimerColorActive(bool active)
    {
        timerImage.color = active ? activeColor : inactiveColor;
    }

}
