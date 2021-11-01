using QuantumTek.QuantumAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public QA_AttributeHandler playerAttributes;


    private void Start()
    {
        
        playerAttributes.AddAttribute("PlayerEnergy");
        playerAttributes.AddAttribute("GameEnergy");
        playerAttributes.SetAttributeValue("PlayerEnergy", 100);
        playerAttributes.SetAttributeValue("GameEnergy", 0);
    }
   
    public void AddGameEnergy(float energyToAdd)
    {
        
        float e = playerAttributes.GetAttributeValue("GameEnergy");
        e += energyToAdd;
        playerAttributes.SetAttributeValue("GameEnergy", e);
    }
    public void AddPlayerEnergy(float energyToAdd)
    {
        float e = playerAttributes.GetAttributeValue("PlayerEnergy");
        e += energyToAdd;
        playerAttributes.SetAttributeValue("PlayerEnergy", e);
    }
    public void RemoveGameEnergy(float energyToAdd)
    {
    
        float e = playerAttributes.GetAttributeValue("GameEnergy");
        e -= energyToAdd;
        playerAttributes.SetAttributeValue("GameEnergy", e);
    }
    public void RemovePlayerEnergy(float energyToAdd)
    {
        float e = playerAttributes.GetAttributeValue("PlayerEnergy");
        e -= energyToAdd;
        playerAttributes.SetAttributeValue("PlayerEnergy", e);
    }
}
