using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionDisplay : MonoBehaviour
{
    public TextMeshProUGUI versionText;

    private void Start()
    {
        versionText.text = $"v {Application.version}";
    }
}
