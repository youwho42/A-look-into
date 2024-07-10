using QuantumTek.QuantumInventory;
using UnityEngine;
using TMPro;
using System.Collections;

public class ItemInformationDisplayUI : MonoBehaviour
{
    public static ItemInformationDisplayUI instance;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public GameObject informationDisplay;
    public TextMeshProUGUI itemName;
    public Vector2 offset;
    public RectTransform canvasRectTransform;
    QI_ItemData currentItem;
    RectTransform otherAnchor;
    bool isShowing;

    private void Start()
    {
        GameEventManager.onInventoryUpdateEvent.AddListener(HideItemName);
        
        informationDisplay.SetActive(false);
    }

    private void OnDisable()
    {
        GameEventManager.onInventoryUpdateEvent.RemoveListener(HideItemName);
    }

    public void ShowItemName(QI_ItemData item, RectTransform anchor)
    {
        currentItem = item;
        otherAnchor = anchor;
        if (isShowing)
        {
            StopCoroutine("HideItemCo");
            ShowItem();
        }
        else
            Invoke("ShowItem", 1.3f);

    }


    public void HideItemName()
    {

        StartCoroutine("HideItemCo");
        informationDisplay.SetActive(false);
        CancelInvoke("ShowItem");
    }

    void ShowItem()
    {
        isShowing = true;
        itemName.text = currentItem.Name;

        // Convert the screen point to a position in the canvas

        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, otherAnchor.position + (Vector3)offset, null, out anchoredPosition);

        // Apply the anchored position to the UI element
        informationDisplay.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        informationDisplay.SetActive(true);
    }

    IEnumerator HideItemCo()
    {
        yield return new WaitForSeconds(0.4f);
        isShowing = false;
    }
}
