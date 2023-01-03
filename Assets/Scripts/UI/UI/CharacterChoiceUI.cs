using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;


public class CharacterChoiceUI : MonoBehaviour
{
    public SpriteResolver chooseSpriteResolver;
    public SpriteResolver playerSpriteResolver;
    public PlayerCharacterManager playerCharacters;
    
    int index = 0;

    public TMP_InputField playerNameInputField;
    public Button acceptButton;
    string spriteName;
    void Start()
    {

        spriteName = playerCharacters.baseCharacters[0];
        chooseSpriteResolver.SetCategoryAndLabel("Player", playerCharacters.baseCharacters[0]);
        //Changes the character limit in the main input field.
        playerNameInputField.characterLimit = 12;
        CheckPlayerNameValid();
    }


    public void ChangeSprite(int dir)
    {
        
        index += dir;
        if (index < 0)
            index = playerCharacters.aquiredCharacters.Count - 1;
        else if(index >=playerCharacters.aquiredCharacters.Count)
            index = 0;
        //index = Mathf.Clamp(index, 0, playerCharacters.aquiredCharacters.Count - 1);
        spriteName = playerCharacters.aquiredCharacters[index];
        chooseSpriteResolver.SetCategoryAndLabel("Player", spriteName);
    }

    public void AcceptPlayerSettings()
    {
        SetPlayerSprite();
        SetPlayerName();
    }
    void SetPlayerSprite()
    {
        playerSpriteResolver.SetCategoryAndLabel("Player", spriteName);
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
}
