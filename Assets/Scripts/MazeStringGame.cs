using Klaxon.MazeTech;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeStringGame : MonoBehaviour
{
    public MazeCreator mazeCreator;
    int currentIndex = -1;
    public LineRenderer line;

    private void Start()
    {
       
        
    }
    [ContextMenu("Reset Line")]
    void ResetLine()
    {
        line.positionCount = mazeCreator.endPosts.Length;
        for (int i = 0; i < line.positionCount; i++)
        {
            line.SetPosition(i, mazeCreator.convertedBasePosition + new Vector3(0,0,7));
        }
        currentIndex = -1;
    }

    [ContextMenu("Set Next Line")]
    void SetLinePosition()
    {
        currentIndex++;
        if(currentIndex < line.positionCount) 
            line.SetPosition(currentIndex, mazeCreator.endPosts[currentIndex].transform.position + mazeCreator.endPosts[currentIndex].lineDisplacement.displacedPosition + new Vector3(0, 0, 2));
    }
}
