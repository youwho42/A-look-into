using Klaxon.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePost : Interactable
{
    public List<GameObject> postSigns = new List<GameObject>();
    [HideInInspector]
    public int postIndex = 0;
    public DrawZasYDisplacement lineDisplacement;
    public MazeStringGame mazeString;
    public FixingSounds attachSound;
    public void SetPostSign(int index)
    {

        for (int i = 0; i < postSigns.Count; i++)
        {
            postSigns[i].SetActive(false);
            if(index == i)
            {
                postSigns[i].SetActive(true);
                postIndex = i + 1;
                
            }
                
        }
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        InteractWithPost();
    }

    void InteractWithPost()
    {
        if (postIndex - 1 == mazeString.currentIndex)
        {
            mazeString.SetLinePosition();
            mazeString.inMaze = true;
            PlayerInformation.instance.inMaze = true;
        }
            
        
    }

    public void StartSounds()
    {
        attachSound.StartSoundsWithTimer();
    }
}
