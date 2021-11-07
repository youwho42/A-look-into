using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSystemManager : MonoBehaviour
{
    public static TeleportSystemManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }


    public List<Transform> allTeleports = new List<Transform>();

}
