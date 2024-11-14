using Klaxon.StatSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifierDisplay : MonoBehaviour
{
    public StatModifier statModifier;
    public TextMeshProUGUI amountText;
    public Image modifierIcon;
    public Image modifierIconGrey;

    
    public void SetModifierUI()
    {
        modifierIcon.sprite = statModifier.modIcon;
        modifierIconGrey.sprite = statModifier.modIconGrey;
        SetTimer();
    }
     
    void SetTimer()
    {
        float current = statModifier.GetTimer();
        float max = statModifier.GetMaxTimer();
        modifierIcon.fillAmount = current / max;
    }

    public void ShowInformation()
    {
        if (statModifier == null)
            return;
        ItemInformationDisplayUI.instance.ShowModifierInfo(statModifier, this.GetComponent<RectTransform>());
    }
    public void HideInformation()
    {
        ItemInformationDisplayUI.instance.HideItemName();
    }

}
