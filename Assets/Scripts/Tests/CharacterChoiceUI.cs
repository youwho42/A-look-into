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

    
    int index = 0;

    public TMP_InputField playerNameInputField;
    string spriteName;
    void Start()
    {
        chooseSpriteResolver.SetCategoryAndLabel("Player", "Entry_0");
        //Changes the character limit in the main input field.
        playerNameInputField.characterLimit = 12;
        
    }


    public void ChangeSprite(int dir)
    {
        index = (index + dir);
        index = Mathf.Clamp(index, 0, 2);
        spriteName = $"Entry_{index}";
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
}
