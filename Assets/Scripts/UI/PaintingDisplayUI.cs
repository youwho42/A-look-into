using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaintingDisplayUI : MonoBehaviour
{
    public static PaintingDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public TextMeshProUGUI paintingTitle;
    public TextMeshProUGUI paintingInfo;
    public Image paintingBG;
    public List<Image> paintingLayers = new List<Image>();
    public Image finalImage;
    RestorePainting currentPainting;

    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.PaintingUI);

        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        currentPainting = null;
    }

    void DisableAllLayers()
    {
        paintingBG.sprite = null;
        foreach (var layer in paintingLayers)
        {
            layer.sprite = null;
            layer.gameObject.SetActive(false);
        }
        finalImage.sprite = null;
        finalImage.gameObject.SetActive(false);

    }

    void AddPainting(RestorePainting painting)
    {
        DisableAllLayers();
        currentPainting = painting;
        paintingTitle.text = currentPainting.painting.localizedName.GetLocalizedString();

        bool isFinished = currentPainting.GetIsFinished();
        paintingInfo.text = isFinished ? currentPainting.painting.localizedDescription.GetLocalizedString() : "...";
        
        paintingBG.sprite = currentPainting.painting.paintingBGSprite;
        
        for (int i = 0; i < currentPainting.ingredients.Count; i++)
        {

            if (currentPainting.ingredients[i].activated)
            {
                int index = currentPainting.GetPaintingLayer(currentPainting.ingredients[i].item);
                if(index != -1)
                {
                    paintingLayers[i].sprite = currentPainting.painting.paintingLayers[index].LayerSprite;
                    paintingLayers[i].gameObject.SetActive(true);
                }
                
            }
        }
        if (isFinished)
        {
            finalImage.sprite = currentPainting.painting.finalLayer;
            finalImage.gameObject.SetActive(true); 
        }
            
    }

    public void ShowUI(RestorePainting painting)
    {
        AddPainting(painting);
    }
    
}
