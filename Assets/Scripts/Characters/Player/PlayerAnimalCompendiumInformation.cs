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
    

    public void AddAnimal(string name)
    {
        if (animalNames.Contains(name))
        {
            int index = animalNames.IndexOf(name);
            animalTimesViewed[index] += 1;
        }
        else
        {
            animalNames.Add(name);
            animalTimesViewed.Add(1);
        }
    }

    public int TimesViewed(string name)
    {
        if (animalNames.Contains(name))
        {
            int index = animalNames.IndexOf(name);
            return animalTimesViewed[index];
        }
        return 0;
    }
}
