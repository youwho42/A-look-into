﻿using QuantumTek.QuantumAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public QA_AttributeHandler playerAttributes;


    private void Start()
    {
        
        playerAttributes.AddAttribute("Bounce");
        playerAttributes.AddAttribute("Agency");
        playerAttributes.AddAttribute("Luck");
        playerAttributes.SetAttributeValue("Bounce", 100);
        playerAttributes.SetAttributeValue("Agency", 0);
        playerAttributes.SetAttributeValue("Luck", 5);
        
    }
   
    public void AddGameEnergy(float energyToAdd)
    {
        
        float e = playerAttributes.GetAttributeValue("Agency");
        e += energyToAdd;
        playerAttributes.SetAttributeValue("Agency", e);
    }
    public void AddPlayerEnergy(float energyToAdd)
    {
        float e = playerAttributes.GetAttributeValue("Bounce");
        e += energyToAdd;
        playerAttributes.SetAttributeValue("Bounce", e);
    }
    public void AddPlayerLuck(float luckToAdd)
    {
        float e = playerAttributes.GetAttributeValue("Luck");
        e += luckToAdd;
        playerAttributes.SetAttributeValue("Luck", e);
    }
    public void RemoveGameEnergy(float energyToAdd)
    {
    
        float e = playerAttributes.GetAttributeValue("Agency");
        e -= energyToAdd;
        playerAttributes.SetAttributeValue("Agency", e);
    }
    public void RemovePlayerEnergy(float energyToAdd)
    {
        float e = playerAttributes.GetAttributeValue("Bounce");
        e -= energyToAdd;
        playerAttributes.SetAttributeValue("Bounce", e);
    }
    public void RemovePlayerLuck(float luckToAdd)
    {
        float e = playerAttributes.GetAttributeValue("Luck");
        e -= luckToAdd;
        playerAttributes.SetAttributeValue("Luck", e);
    }

    public float GetPlayerLuck(float luckToAdd)
    {
        float l = (luckToAdd / 100) * playerAttributes.GetAttributeValue("Luck");
        return luckToAdd + l;
    }
}