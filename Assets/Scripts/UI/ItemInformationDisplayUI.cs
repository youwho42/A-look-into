using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemInformationDisplayUI : MonoBehaviour
{
    public static ItemInformationDisplayUI instance;
    public GameObject informationDisplay;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public Vector2 offset;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        informationDisplay.SetActive(false);
    }
    public void ShowInformationDisplay(QI_ItemData item)
    {
        itemName.text = item.Name;
        //itemDescription.text = item.Description;
        
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        transform as RectTransform,
        Input.mousePosition, Camera.current,
        out movePos);
        movePos += offset;
        informationDisplay.transform.localPosition = movePos;
        informationDisplay.SetActive(true);
    }
    public void HideInformationDisplay()
    {
        informationDisplay.SetActive(false);
    }

}
