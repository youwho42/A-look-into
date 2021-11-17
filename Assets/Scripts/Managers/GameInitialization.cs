
using UnityEngine;

public class GameInitialization
{


    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        RandomPlantGrowthAtStart.instance.SetPlantGrowthAtStart();
    }

    /*[RuntimeInitializeOnLoadMethod]
    static void OnSecondRuntimeMethodLoad()
    {
        Debug.Log("SecondMethod After Scene is loaded and game is running.");
    }*/

}