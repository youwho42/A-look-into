using Klaxon.MazeTech;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeStringGame : MonoBehaviour
{
    public MazeCreator mazeCreator;
    [HideInInspector]
    public int currentIndex = -1;
    public LineRenderer line;
    public List<MazePost> endPostItems = new List<MazePost>();
    [HideInInspector]
    public bool inMaze;
    [HideInInspector]
    public bool mazeComplete;
    public QI_ItemDatabase mazeItemDatabase;
    public QI_Inventory rewardBox;

    public void ResetLine()
    {
        inMaze = false;
        line.positionCount = 0;
        mazeComplete = false;
        currentIndex = -1;
    }

    private void Update()
    {
        if(inMaze && line.positionCount > 1 && !mazeComplete) 
            line.SetPosition(line.positionCount - 1, PlayerInformation.instance.player.position + new Vector3(0, 0.2990625f*0.6f, endPostItems[currentIndex].transform.position.z + 2));
    }

    
    public void SetLinePosition()
    {
        currentIndex++;
        line.positionCount = currentIndex + 2;
        line.positionCount = Mathf.Clamp(line.positionCount, 0, endPostItems.Count + 1);
        if (currentIndex < line.positionCount)
            line.SetPosition(currentIndex, endPostItems[currentIndex].transform.position + endPostItems[currentIndex].lineDisplacement.displacedPosition + new Vector3(0, 0, 2));

        if (currentIndex == endPostItems.Count - 1)
        {
            line.positionCount = currentIndex + 1;
            mazeComplete = true;
            inMaze = false;
            rewardBox.AddItem(mazeItemDatabase.GetRandomWeightedItem(), 1, false);
        }
            
    }
}
