using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FusedVR.Web3;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AuthorizeNFT : MonoBehaviour
{
    public string paintingName;
    public TMP_InputField inputFieldEmail;

    public GameObject alertBox;
    public TextMeshProUGUI alertText;
    public bool verified;

    public TextMeshProUGUI loadingText;
    public Slider loadScreenSlider;
    public GameObject startAuthScreen;
    public GameObject loadScreen;
    public Camera loadCamera;
    public Button continueButton;
    public Button saveButton;
    public Toggle saveToggle;

    public string alertEmailError;
    public string alertEmailSent;
    public string alertPleaseVerify;
    public string alertVerifyBeforeSave;
    public string alertVerificationsaved;
   
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        
    }
        private void Start()
    {
        verified = false;
        
        
        SavingLoading.instance.Load();
        continueButton.interactable = verified;
        saveButton.interactable = verified;
        saveToggle.isOn = verified;
    }

    public void SaveVerified()
    {
        if (verified)
        {
            SavingLoading.instance.Save();
            ShowAlertBox(alertVerificationsaved);
            saveToggle.isOn = true;
        }
        else
            ShowAlertBox(alertVerifyBeforeSave);
    }
    public void SendMail()
    {
        if (inputFieldEmail.text.Contains("@") && inputFieldEmail.text.Contains("."))
        {
            SendVerificationMail();
            ShowAlertBox(alertEmailSent);
        }
        else
        {
            ShowAlertBox(alertEmailError);
        }
    }

    void ShowAlertBox(string alert)
    {
        alertBox.SetActive(true);
        alertText.text = alert;
    }

    

    public void CloseAlertBox()
    {
        alertText.text = "";
        alertBox.SetActive(false);
    }

    public void Continue()
    {

        if (verified)
            StartCoroutine(LoadPainting(paintingName));
        else
            ShowAlertBox(alertPleaseVerify);
    }
    public void Quit()
    {
        Application.Quit();
    }

    async void SendVerificationMail()
    {
        if (await Web3Manager.Login(inputFieldEmail.text, "id"))
        {
            // check if nft is in said wallet
            verified = true;
            continueButton.interactable = true;
            saveButton.interactable = true;
        }
    }

    

    IEnumerator LoadPainting(string levelName)
    {
        loadScreen.SetActive(true);
        startAuthScreen.SetActive(false);
        AsyncOperation currentLevelLoading = SceneManager.LoadSceneAsync(levelName);


        while (!currentLevelLoading.isDone)
        {

            float progress = Mathf.Clamp(currentLevelLoading.progress / 0.9f, 0, 1);

            loadScreenSlider.value = progress;
            loadingText.text = $"Loading scene: {Mathf.RoundToInt(progress * 100)}%";


            yield return null;
        }


        yield return new WaitForSeconds(0.5f);

        loadingText.text = "Thank you for waiting.";

        yield return new WaitForSeconds(5f);

        loadScreen.SetActive(false);
      
        

        yield return null;

    }

    
}
