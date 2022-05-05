
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractAreasManager : MonoBehaviour
{
    public SpotType spotType;
    
    public List<DrawZasYDisplacement> allAreas = new List<DrawZasYDisplacement>();

    void Start()
    {
        var all = FindObjectsOfType<DrawZasYDisplacement>();
        foreach (var item in all)
        {
            if (item.spotType == spotType)
            {
                allAreas.Add(item);
            }
        }
    }
}
