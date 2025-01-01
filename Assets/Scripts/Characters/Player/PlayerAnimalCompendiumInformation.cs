using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAnimalCompendiumInformation : MonoBehaviour
{
    
    [HideInInspector]
    public List<string> animalNames = new List<string>();
    [HideInInspector]
    public List<int> animalTimesViewed = new List<int>();
    [HideInInspector]
    public List<bool> viewedComplete = new List<bool>();

    public void AddAnimal(string name, int amount)
    {
        if (animalNames.Contains(name))
        {
            int index = animalNames.IndexOf(name);
            animalTimesViewed[index] += amount;
        }
        else
        {
            animalNames.Add(name);
            animalTimesViewed.Add(amount);
            viewedComplete.Add(false);
        }
    }

    public void SetViewedComplete(int index)
    {
        viewedComplete[index] = true;
    }
    public int GetTimesViewed(string name)
    {
        if (animalNames.Contains(name))
        {
            int index = animalNames.IndexOf(name);
            return animalTimesViewed[index];
        }
        return 0;
    }
}
