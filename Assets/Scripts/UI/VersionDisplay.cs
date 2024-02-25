using Klaxon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    private void Start()
    {
        versionText.text = $"v {Application.version}";
        
    }

    public bool CompareVersions()
    {
        if (savedVersion == "")
            return true;
        return savedVersion == Application.version;
    }
}
