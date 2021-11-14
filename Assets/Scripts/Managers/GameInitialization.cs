
using UnityEngine;

public class GameInitialization
{


    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        Debug.Log("SecondMethod After Scene is loaded and game is running.");
        RandomPlantGrowthAtStart.instance.SetPlantGrowthAtStart();
    }

    /*[RuntimeInitializeOnLoadMethod]
    static void OnSecondRuntimeMethodLoad()
    {
        Debug.Log("SecondMethod After Scene is loaded and game is running.");
    }*/

}