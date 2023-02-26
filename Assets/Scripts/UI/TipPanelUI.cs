using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipPanelUI : MonoBehaviour
{
    

    public TextMeshProUGUI tipTextField;

    public void SetTipPanel(string tipText)
    {
        tipTextField.text = tipText;
    }
}
