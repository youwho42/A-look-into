using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportDisplayUI : MonoBehaviour
{

    public static TeleportDisplayUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public GameObject teleportUI;
    TeleportSystemManager teleportSystem;
    PlayerInformation playerInformation;

    public GameObject destinationListHolder;
    public DestinationButton destinationButton;

    private void Start()
    {
        teleportSystem = TeleportSystemManager.instance;
        playerInformation = PlayerInformation.instance;
        teleportUI.SetActive(false);
    }
    public void ShowUI(Teleport teleport)
    {
        UpdateDestinationList(teleport);
        playerInformation.TogglePlayerInput(false);
        //teleportUI.SetActive(true);
    }

    public void HideUI()
    {
        playerInformation.TogglePlayerInput(true);
        //teleportUI.SetActive(false);
    }
    void UpdateDestinationList(Teleport teleport)
    {
        ClearDestinationSlot();
        for (int i = 0; i < teleportSystem.allTeleports.Count; i++)
        {
            if(teleportSystem.allTeleports[i] != teleport.transform) 
            { 
                DestinationButton newDestination = Instantiate(destinationButton, destinationListHolder.transform);
                newDestination.AddDestination(teleportSystem.allTeleports[i], teleport);
            }
        }
    }


    public void ClearDestinationSlot()
    {
        while (destinationListHolder.transform.childCount > 0)
        {
            DestroyImmediate(destinationListHolder.transform.GetChild(0).gameObject);
        }
    }
}
