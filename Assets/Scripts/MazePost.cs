using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePost : MonoBehaviour
{
    public List<GameObject> postSigns = new List<GameObject>();
    [HideInInspector]
    public int postIndex;
    public DrawZasYDisplacement lineDisplacement;

    public void SetPostSign(int index)
    {

        for (int i = 0; i < postSigns.Count; i++)
        {
            postSigns[i].SetActive(false);
            if(index == i)
            {
                postSigns[i].SetActive(true);
                postIndex = i;
            }
                
        }
    }
}
