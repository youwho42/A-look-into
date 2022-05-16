using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAuth : MonoBehaviour, ISaveable
{
    AuthorizeNFT auth;

    
    void Start()
    {
       
    }
    
    public object CaptureState()
    {
        return new SaveData
        {
            saveAuth = auth.verified
            
        };
    }

    public void RestoreState(object state)
    {
        auth = GetComponent<AuthorizeNFT>();
        var saveData = (SaveData)state;
        auth.verified = saveData.saveAuth;

    }

    [Serializable]
    private struct SaveData
    {
        public bool saveAuth;
        
    }
}
