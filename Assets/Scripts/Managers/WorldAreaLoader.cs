using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldAreaLoader : MonoBehaviour
{

    public string decoSceneToLoad;


  

    public IEnumerator LoadArea()
    {
        
           
        AsyncOperation asyncLoadDeco = SceneManager.LoadSceneAsync(decoSceneToLoad, LoadSceneMode.Additive);
        
        yield return asyncLoadDeco;
        
    }

    public IEnumerator UnloadArea()
    {
        
        AsyncOperation asyncUnloadArea = SceneManager.UnloadSceneAsync(decoSceneToLoad);

        yield return asyncUnloadArea;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Scene areaToLoad = SceneManager.GetSceneByName(decoSceneToLoad);
            if (!areaToLoad.isLoaded)
                StartCoroutine(LoadArea());
        }
            
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Scene areaToLoad = SceneManager.GetSceneByName(decoSceneToLoad);
            if (areaToLoad.isLoaded)
                StartCoroutine(UnloadArea());
        }
            
    }

    

}
