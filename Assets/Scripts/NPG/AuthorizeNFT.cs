using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FusedVR.Web3;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;

public class AuthorizeNFT : MonoBehaviour
{
    //public string paintingName;
    //string contract = "0x20536AaBb2f8BC36420E0424404A16739ABAA8E3";
    //public string token_id;

    //public TMP_InputField inputFieldEmail;

    //public GameObject alertBox;
    //public TextMeshProUGUI alertText;
    //public bool verified;

    //public TextMeshProUGUI loadingText;
    //public Slider loadScreenSlider;
    //public GameObject startAuthScreen;
    //public GameObject loadScreen;
    //public Camera loadCamera;
    //public Button continueButton;
    //public Button saveButton;
    //public Button deleteSaveButton;

    //public string alertEmailError;
    //public string alertEmailSent;
    //public string alertPleaseVerify;
    //public string alertVerifyBeforeSave;
    //public string alertVerificationFailed;
    //public string alertVerificationSaved;
    //[HideInInspector]
    //public string dateSaved;
    //DateTime currentDate;
    //public Toggle fullscreen;


  
    //private void Awake()
    //{
    //    DontDestroyOnLoad(this);
        
    //}
    //private void Start()
    //{
    //    fullscreen.isOn = Screen.fullScreen;
    //    verified = false;
    //    saveButton.interactable = false;
    //    deleteSaveButton.interactable = false;
    //    SavingLoading.instance.Load();
        
    //    continueButton.interactable = verified;

    //    SetSaveDeleteButtons();
    //}

    //public void SetFullScreen()
    //{
    //    Screen.fullScreen = fullscreen.isOn;
    //}

    //public void SetSaveDeleteButtons()
    //{
    //    if (!SavingLoading.instance.SaveExists() && verified)
    //        saveButton.interactable = true;
    //    else
    //        saveButton.interactable = false;
        
    //    deleteSaveButton.interactable = SavingLoading.instance.SaveExists();
        
    //}
    //public void DeleteVerifiedState()
    //{
    //    verified = false;
    //    saveButton.interactable = false;
    //}

    //public void SaveVerifiedAndDate()
    //{
    //    if (verified)
    //    {
    //        dateSaved = DateTime.Now.AddDays(30).ToString();
    //        SavingLoading.instance.Save();
    //        ShowAlertBox(alertVerificationSaved);
    //        SetSaveDeleteButtons();
    //    }
    //    else
    //        ShowAlertBox(alertVerifyBeforeSave);
    //}
    //public void SendMail()
    //{
    //    if (inputFieldEmail.text.Contains("@") && inputFieldEmail.text.Contains("."))
    //    {
    //        SendVerificationMail();
    //        ShowAlertBox(alertEmailSent);
    //    }
    //    else
    //    {
    //        ShowAlertBox(alertEmailError);
    //    }
    //}

    //void ShowAlertBox(string alert)
    //{
    //    alertBox.SetActive(true);
    //    alertText.text = alert;
    //}

    

    //public void CloseAlertBox()
    //{
    //    alertText.text = "";
    //    alertBox.SetActive(false);
    //}

    //public void Continue()
    //{

    //    if (verified)
    //        StartCoroutine(LoadPainting(paintingName));
    //    else
    //        ShowAlertBox(alertPleaseVerify);
    //}
    //public void Quit()
    //{
    //    Application.Quit();
    //}

    //async void SendVerificationMail()
    //{

    //    if (await Web3Manager.Login(inputFieldEmail.text, "id"))
    //    {

    //        var ethTokens = await Web3Manager.GetNFTTokens("eth");
    //        bool success = false;
    //        foreach (Dictionary<string, string> nft in ethTokens)
    //        {
    //            if (nft.ContainsValue(contract.ToLower()))
    //            {
    //                if (nft.TryGetValue("token_id", out string id))
    //                {
    //                    if (id == token_id)
    //                    {
    //                        verified = true;
    //                        continueButton.interactable = true;
    //                        SetSaveDeleteButtons();
    //                        success = true;
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //        if (!success)
    //            ShowAlertBox(alertVerificationFailed);
    //    }
    //    else
    //    {
    //        ShowAlertBox(alertVerificationFailed);
    //    }
    //}

    

    //IEnumerator LoadPainting(string levelName)
    //{
    //    loadScreen.SetActive(true);
    //    startAuthScreen.SetActive(false);
    //    AsyncOperation currentLevelLoading = SceneManager.LoadSceneAsync(levelName);


    //    while (!currentLevelLoading.isDone)
    //    {

    //        float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

    //        loadScreenSlider.value = progress;
    //        loadingText.text = $"Loading scene: {Mathf.RoundToInt(progress * 100)}%";


    //        yield return null;
    //    }


    //    yield return new WaitForSeconds(0.5f);

    //    loadingText.text = "Thank you for waiting.";

    //    yield return new WaitForSeconds(5f);

    //    loadScreen.SetActive(false);
      
        

    //    yield return null;

    //}

    
}
