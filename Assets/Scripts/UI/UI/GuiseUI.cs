using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class GuiseUI : MonoBehaviour
{
    public static GuiseUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public SpriteResolver chooseSpriteResolver;
    //public SpriteResolver playerSpriteResolver;

    public GameObject characterChoiceCameraObject;

    int index = 0;

    string spriteName;
    public SetButtonSelected setButtonSelected;

    void Start()
    {

        //spriteName = PlayerInformation.instance.characterManager.baseCharacters[0];
        chooseSpriteResolver.SetCategoryAndLabel("Player", PlayerInformation.instance.characterManager.baseCharacters[0]);
        //Changes the character limit in the main input field.
        
        HideUI();
    }

    public void ShowUI()
    {
        characterChoiceCameraObject.SetActive(true);
    }
    public void HideUI()
    {
        characterChoiceCameraObject.SetActive(false);
    }

    public void ChangeSprite(int dir)
    {

        index += dir;
        if (index < 0)
            index = PlayerInformation.instance.characterManager.aquiredCharacters.Count - 1;
        else if (index >= PlayerInformation.instance.characterManager.aquiredCharacters.Count)
            index = 0;
        //index = Mathf.Clamp(index, 0, playerCharacters.aquiredCharacters.Count - 1);
        spriteName = PlayerInformation.instance.characterManager.aquiredCharacters[index];
        chooseSpriteResolver.SetCategoryAndLabel("Player", spriteName);
    }

    public void AcceptPlayerSettings()
    {
        SetPlayerSprite();
        CancelSelection();
    }
    
    public void CancelSelection()
    {
        UIScreenManager.instance.HideAllScreens();
        UIScreenManager.instance.DisplayPlayerHUD(LevelManager.instance.HUDBinary == 1);

        HideUI();
    }
    void SetPlayerSprite()
    {
        PlayerInformation.instance.playerSpriteResolver.SetCategoryAndLabel("Player", spriteName);
    }
    
    
}
