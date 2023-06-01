using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;


public class CharacterChoiceUI : MonoBehaviour
{
    public SpriteResolver chooseSpriteResolver;
    //public SpriteResolver playerSpriteResolver;
    
    
    int index = 0;

    public TMP_InputField playerNameInputField;
    public Button acceptButton;
    string spriteName;
    public SetButtonSelected setButtonSelected;
    void Start()
    {

        spriteName = PlayerInformation.instance.characterManager.baseCharacters[0];
        chooseSpriteResolver.SetCategoryAndLabel("Player", PlayerInformation.instance.characterManager.baseCharacters[0]);
        //Changes the character limit in the main input field.
        playerNameInputField.characterLimit = 12;
        CheckPlayerNameValid();
    }


    public void ChangeSprite(int dir)
    {
        
        index += dir;
        if (index < 0)
            index = PlayerInformation.instance.characterManager.aquiredCharacters.Count - 1;
        else if(index >= PlayerInformation.instance.characterManager.aquiredCharacters.Count)
            index = 0;
        //index = Mathf.Clamp(index, 0, playerCharacters.aquiredCharacters.Count - 1);
        spriteName = PlayerInformation.instance.characterManager.aquiredCharacters[index];
        chooseSpriteResolver.SetCategoryAndLabel("Player", spriteName);
    }

    public void AcceptPlayerSettings()
    {
        SetPlayerSprite();
        SetPlayerName();
        LevelManager.instance.PlayerSpriteAndNameAccepted();
    }
    public void CancelSelection()
    {
        UIScreenManager.instance.DisplayScreen(UIScreenType.StartScreen);
    }
    void SetPlayerSprite()
    {
        PlayerInformation.instance.playerSpriteResolver.SetCategoryAndLabel("Player", spriteName);
    }
    void SetPlayerName()
    {
        PlayerInformation.instance.SetPlayerName(playerNameInputField.text);
    }
    public void CheckPlayerNameValid()
    {
        if(playerNameInputField.text == "")
            acceptButton.interactable = false;
        else
            acceptButton.interactable = true;
    }

    public void CharacterNameEnter()
    {
        setButtonSelected.SetSelectedButton();
    }
}
