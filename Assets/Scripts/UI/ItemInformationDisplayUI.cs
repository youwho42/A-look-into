using QuantumTek.QuantumInventory;
using UnityEngine;
using TMPro;
using System.Collections;
using Klaxon.StatSystem;

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
    public Vector2 itemAnchorOffset;
    public RectTransform canvasRectTransform;
    QI_ItemData currentItem;
    RectTransform otherItemAnchor;
    bool isShowing;
    StatModifier currentStat;
    RectTransform otherStatAnchor;
    public Vector2 statAnchorOffset;

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
        otherItemAnchor = anchor;
        if (isShowing)
        {
            StopCoroutine("HideItemCo");
            ShowItem();
        }
        else
            Invoke("ShowItem", 1.3f);

    }

    public void ShowModifierInfo(StatModifier stat, RectTransform anchor)
    {
        currentStat = stat;
        otherStatAnchor = anchor;
        if (isShowing)
        {
            StopCoroutine("HideItemCo");
            ShowStat();
        }
        else
            Invoke("ShowStat", 1.3f);

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
        itemName.text = currentItem.localizedName.GetLocalizedString();

        // Convert the screen point to a position in the canvas

        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, otherItemAnchor.position + (Vector3)itemAnchorOffset, null, out anchoredPosition);

        // Apply the anchored position to the UI element
        informationDisplay.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        informationDisplay.SetActive(true);
    }

    void ShowStat()
    {
        isShowing = true;
        itemName.text = currentStat.EffectDescription.GetLocalizedString();

        // Convert the screen point to a position in the canvas

        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, otherStatAnchor.position + (Vector3)statAnchorOffset, null, out anchoredPosition);

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
