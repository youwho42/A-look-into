using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PaintingRestorationUI : MonoBehaviour
{

    public static PaintingRestorationUI instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public TextMeshProUGUI title;
    public GameObject compendiumHolder;
    public GameObject itemHolder;
    public RestorePaintingSlot compendiumSlot;
    public RestorePaintingSlot itemSlot;

    List<RestorePaintingSlot> slots = new List<RestorePaintingSlot>(); 
     
    RestorePainting currentPainting;

    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.RestorePaintingUI);

        gameObject.SetActive(false);
    }

    public void ShowUI(RestorePainting painting)
    {
        currentPainting = painting;
        title.text = currentPainting.painting.localizedName.GetLocalizedString();
        ClearSlots();
        foreach (var item in currentPainting.ingredients)
        {
            
            var slot = Instantiate(item.isPhysicalItem ? itemSlot: compendiumSlot, item.isPhysicalItem ? itemHolder.transform : compendiumHolder.transform);
            slots.Add(slot);
            slot.AddItem(item);
        }
    }

    public void HideUI()
    {
        currentPainting = null;
        GameEventManager.onMuseumPieceUpdateEvent.Invoke();
    }

    public void ClearSlots()
    {
        foreach (Transform child in compendiumHolder.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in itemHolder.transform)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();
    }
}