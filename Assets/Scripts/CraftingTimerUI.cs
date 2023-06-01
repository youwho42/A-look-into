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
        timerCanvas.enabled = true;
    }

    public void HideTimer()
    {
        timerCanvas.enabled = false;
    }
   

    public void SetCraftingTimer(int time, int maxTime)
    {
        
        timerCanvas.gameObject.SetActive(true);
        timerImage.fillAmount = MapNumber.Remap(time-1, 0, maxTime, 0, 1);
    }
}
