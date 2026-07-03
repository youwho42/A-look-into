using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReplaceObjectOnItemDrop : MonoBehaviour
{
   
    public List<GameObject> grassObjects = new List<GameObject>();
    public Vector2 replaceOffset;
    public float replaceRadius = 1f;
    DecorationManager decorationManager;

    private void OnEnable()
    {

        decorationManager = DecorationManager.instance;

        decorationManager.RegisterDecoration(this);

        CheckForObjects();
    }
    private void OnDisable()
    {
        
        ShowObjects(true);
        decorationManager.UnRegisterDecoration(this);
        
    }
    private void OnDestroy()
    {
        var all = decorationManager.GetReplaceObjects(transform.position + (Vector3)replaceOffset, replaceRadius);
        foreach (var each in all)
        {
            each.CheckForObjects();
        }
    }
    public void CheckForObjects()
    {
        var grass = decorationManager.GetDecorations(DecorationType.Grass, transform.position + (Vector3)replaceOffset, replaceRadius);
        var flowers = decorationManager.GetDecorations(DecorationType.Flower, transform.position + (Vector3)replaceOffset, replaceRadius);

        ShowObjects(true);

        var combined = new List<GameObject>(grass.Count + flowers.Count);
        combined.AddRange(grass.Select(item => item.transform.parent.gameObject));
        combined.AddRange(flowers.Select(item => item.transform.parent.gameObject));

        grassObjects.AddRange(combined);

        ShowObjects(false);
    }

    
    public void ShowObjects(bool showState)
    {
        
        foreach (var item in grassObjects)
        {
            if (item != null)
                item.SetActive(showState);
        }
        
        if(showState)
            grassObjects.Clear();
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)replaceOffset, replaceRadius);
    }

}
