using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationDisplayObject : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    public Image image;
    public void SetDisplay(string text, Color color)
    {
        displayText.text = text;
        image.color = color;
        
    }

    
}
