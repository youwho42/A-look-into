using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum NeighborObjectType{
    None,
    Bush,
    Pine,
    SquareLeaf,
    Columnar,
    SmallLeaf,
    Stone,
    Chalcocite,
    Hematite,
    Sperrylite,
    Clay, 
    DeadTree,
    CrimsonKing
}

public class NeighboringObject : MonoBehaviour
{
    public NeighborObjectType NeighborObjectType;

}
