using Klaxon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class VersionDisplay : MonoBehaviour
{
    public static VersionDisplay instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    public TextMeshProUGUI versionText;
    public string savedVersion;

    public int[] intVersion;
    

    private void Start()
    {
        versionText.text = $"v {Application.version}";
    }

    public void SetVersion(int[] intV)
    {
        intVersion = intV;
    }

    public bool CompareVersions()
    {
        if (savedVersion == "" || savedVersion == Application.version)
            return true;

        return false;
        
    }
}
