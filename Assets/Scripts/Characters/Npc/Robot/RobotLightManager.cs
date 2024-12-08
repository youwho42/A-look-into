using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotLightManager : MonoBehaviour
{
    QI_Inventory inventory;
    // Inventory slot lights system
    public List<RobotLight> inventoryLights = new List<RobotLight>();
    public List<InventoryLightSystem> inventoryLightStates = new List<InventoryLightSystem>();

    [Serializable]
    public struct InventoryLightSystem
    {
        public Color color;
        public float intensity;
        public string state;
    }

    // Robot function lights system
    public RobotLight functionLight;
    public List<StateLightSystem> functionLights = new List<StateLightSystem>();
    RobotStates lightstate;
    [Serializable]
    public struct StateLightSystem
    {
        public Color color;
        public float intensity;
        public RobotStates state;
    }

    public RobotStates currentState;
    public enum RobotStates
    {
        Open,
        Waiting,
        Roaming,
        Deviate,
        Gathering,
        Retiring,
        Deactivated

    }

    private IEnumerator Start()
    {
        inventory = GetComponent<QI_Inventory>();
        yield return new WaitForSeconds(0.5f);
        SetInventoryLights();
        SetFunctionLight();
    }

    public void SetInventoryLights()
    {

        for (int i = 0; i < inventoryLights.Count; i++)
        {
            SetInventoryLightState(inventoryLights[i], "Empty");

        }

        for (int i = 0; i < inventory.Stacks.Count; i++)
        {
            string lightState = "Holding";
            if (inventory.Stacks[i].Item.MaxStack > 0 && inventory.Stacks[i].Amount >= inventory.Stacks[i].Item.MaxStack)
                lightState = "Full";



            SetInventoryLightState(inventoryLights[i], lightState);


        }

    }
    void SetInventoryLightState(RobotLight light, string lightState)
    {
        foreach (var state in inventoryLightStates)
        {
            if (state.state == lightState)
            {
                light.SetColor(state.color);
                light.SetIntensity(state.intensity);
            }

        }
    }

    public void SetCurrentFunction(RobotStates state)
    {
        currentState = state;
        SetFunctionLight();
    }

    void SetFunctionLight()
    {
        foreach (var light in functionLights)
        {
            if (light.state == currentState)
            {
                functionLight.SetColor(light.color);
                functionLight.SetIntensity(light.intensity);
                lightstate = light.state;
                return;
            }
        }
    }




}
