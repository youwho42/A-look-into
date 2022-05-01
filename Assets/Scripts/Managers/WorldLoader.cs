using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldLoader : MonoBehaviour
{
    //public List<WorldAreaLoader> areaLoaders = new List<WorldAreaLoader>();


    private void Start()
    {
        /*var areas = FindObjectsOfType<WorldAreaLoader>();
        foreach (var area in areas)
        {
            areaLoaders.Add(area);
        }*/

        AsyncOperation asyncLoadMain = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive);

        

    }

    /*private void Update()
    {
        foreach (var area in areaLoaders)
        {

            if (area.isInArea)
                Debug.Log("Should Load");
            else
                Debug.Log("Should Unload");

        }
    }*/
}
