using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapLegend : MonoBehaviour
{
    public Image pixelColorImage;
    public TextMeshProUGUI legendName;

    public void SetLegend(Color color, string name)
    {
        pixelColorImage.color = color;
        legendName.text = name;
    }
}
