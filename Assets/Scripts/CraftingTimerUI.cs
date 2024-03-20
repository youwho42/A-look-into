using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTimerUI : MonoBehaviour
{
   
    public Image timerImage;
    public Canvas timerCanvas;

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
        timerImage.fillAmount = MapNumber.Remap(time-1, 0, maxTime, 0, 1);
    }
}
