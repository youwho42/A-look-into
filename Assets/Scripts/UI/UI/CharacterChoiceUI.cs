using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using System.Linq;

public class CharacterChoiceUI : MonoBehaviour
{
    public static CharacterChoiceUI instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public SpriteResolver chooseSpriteResolver;
    //public SpriteResolver playerSpriteResolver;

    public GameObject characterChoiceCameraObject;
    
    int index = 0;

    public TMP_InputField playerNameInputField;
    
    public Button acceptButton;
    string spriteName;
    // public SetButtonSelected setButtonSelected;
    public Color selectedColor;
    UIScreen screen;

    private void Start()
    {
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.CharacterSelectUI);
    
        spriteName = PlayerInformation.instance.characterManager.baseCharacters[0];
        chooseSpriteResolver.SetCategoryAndLabel("Player", PlayerInformation.instance.characterManager.baseCharacters[0]);
        //Changes the character limit in the main input field.
        playerNameInputField.characterLimit = 16;

        CheckPlayerNameValid();
        characterChoiceCameraObject.SetActive(false);
        gameObject.SetActive(false);
       
    }
    private void OnEnable()
    {
        playerNameInputField.text = "";
        CheckPlayerNameValid();
        characterChoiceCameraObject.SetActive(true);
        SetRandomCharacter();
        ChangeSprite(1);
    }
    public void OnDisable()
    {
        characterChoiceCameraObject.SetActive(false);
    }
    void SetRandomCharacter()
    {
        int r = Random.Range(0, PlayerInformation.instance.characterManager.baseCharacters.Count);
        index = r;
        spriteName = PlayerInformation.instance.characterManager.baseCharacters[index];
        chooseSpriteResolver.SetCategoryAndLabel("Player", spriteName);
        
    }
    public void ChangeSprite(int dir)
    {
        
        index += dir;
        if (index < 0)
            index = PlayerInformation.instance.characterManager.baseCharacters.Count - 1;
        else if(index >= PlayerInformation.instance.characterManager.baseCharacters.Count)
            index = 0;
        //index = Mathf.Clamp(index, 0, playerCharacters.aquiredCharacters.Count - 1);
        spriteName = PlayerInformation.instance.characterManager.baseCharacters[index];
        chooseSpriteResolver.SetCategoryAndLabel("Player", spriteName);
    }

    public void AcceptPlayerSettings()
    {
        SetPlayerSprite();
        SetPlayerName();
        LevelManager.instance.PlayerSpriteAndNameAccepted();
        UIScreenManager.instance.inMainMenu = false;
        UIScreenManager.instance.HideScreenUI();
    }
    public void CancelSelection()
    {
        UIScreenManager.instance.HideScreenUI();
        UIScreenManager.instance.DisplayScreenUI(UIScreenType.MainMenuUI, true);
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
        bool valid = playerNameInputField.text.All(c => char.IsLetterOrDigit(c));
        if (string.IsNullOrEmpty(playerNameInputField.text) || !valid)
            acceptButton.interactable = false;
        else
            acceptButton.interactable = true;
        ColorBlock block = playerNameInputField.colors;
        block.selectedColor = valid ? selectedColor : Color.red;
        playerNameInputField.colors = block;
    }

    //public void CharacterNameEnter()
    //{
    //    setButtonSelected.SetSelectedButton();
    //}
}
