using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchStationInventorySlot : MonoBehaviour
{
    QI_ItemData item;
    public Image icon;
    public Button button;
    TutorialUI tutorial;

    private void Start()
    {
        tutorial = GetComponentInParent<TutorialUI>();
        button.onClick.AddListener(SetTutorial);
    }


    public void AddItem(QI_ItemData newItem)
    {
        item = newItem;
        icon.sprite = item.Icon;
        icon.enabled = true;

    }

  

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;

    }

    public void AddItemToResearchSlot()
    {
        ResearchStationDisplayUI.instance.researchSlot.AddItem(item);
    }

    private void SetTutorial()
    {
        tutorial.SetNextTutorialIndex(0);
    }
}
