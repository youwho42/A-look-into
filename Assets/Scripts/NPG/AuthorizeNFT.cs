using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FusedVR.Web3;

public class AuthorizeNFT : MonoBehaviour
{
   async void Start()
    {
        if(await Web3Manager.Login("youwho42@gmail.com", "id"))
        {
            string balance = await Web3Manager.GetNativeBalance("eth");
            
        }
    }

}
