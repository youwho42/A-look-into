using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VillageDeskAreaButton : MonoBehaviour
{
    public Button button;
    FixVillageArea currentArea;

    public void SetCurrentVillageArea()
    {
        VillageDeskDisplayUI.instance.SetCurrentVillageArea(currentArea);
    }

    public void AddVillageArea(FixVillageArea newArea)
    {
        currentArea = newArea;
        button.image.sprite = newArea.areaIcon;
    }

    public void Clear()
    {
        currentArea = null;
        button.image.sprite = null;
    }

}
