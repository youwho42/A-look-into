using System.Collections.Generic;
using UnityEngine;

public class JunkPile : MonoBehaviour
{
    public GameObject singleDirtPile;
    public float dirtPileRadius;
    public float dirtPileInterDistance;
    public List<GameObject> pileItems = new List<GameObject>();
    public float itemInterDistance;
    public Transform junkHolder;
    
    

    [ContextMenu("Set Dirt Piles")]
    private void SetDirtPiles()
    {
        RemoveAllObjects(junkHolder);
        Vector2 area = new Vector2(dirtPileRadius * 2, dirtPileRadius * 1.25f);
        SetDirt(area);
        SetItems(area);
    }

    private void SetDirt(Vector2 area)
    {
        
        var pilePositions = PoissonDiscSampler.GeneratePoints(dirtPileInterDistance, area);

        for (int i = 0; i < pilePositions.Count; i++)
        {
            if (Vector2.Distance(pilePositions[i], area * 0.5f) < dirtPileRadius)
            {
                var go = Instantiate(singleDirtPile, junkHolder);
                go.transform.localPosition = pilePositions[i] - (area * 0.5f);
            }
        }

    }
    private void SetItems(Vector2 area)
    {

        var pilePositions = PoissonDiscSampler.GeneratePoints(itemInterDistance, area);
        List<int> availableIndex = ResetAvailableIndex();

        
        for (int i = 0; i < pilePositions.Count; i++)
        {
            if (Vector2.Distance(pilePositions[i], area * 0.5f) < dirtPileRadius)
            {
                if(availableIndex.Count<=0)
                    availableIndex = ResetAvailableIndex();
                int ind = Random.Range(0, availableIndex.Count);
                int index = availableIndex[ind];
                availableIndex.RemoveAt(ind);
                    
                

                var go = Instantiate(pileItems[index], junkHolder);
                go.transform.localPosition = pilePositions[i] - (area * 0.5f);
            }
        }

    }

    private List<int> ResetAvailableIndex()
    {
        List<int> index = new List<int>();
        for (int i = 0; i < pileItems.Count; i++)
        {
            index.Add(i);
        }
        return index;
    }

    public void RemoveAllObjects(Transform parent)
    {
        //Get an array with all children to this transform
        GameObject[] allItemToDelete = GetAllChildren(parent);


        //Now destroy them
        foreach (GameObject child in allItemToDelete)
        {
            DestroyImmediate(child);
        }

    }
    private GameObject[] GetAllChildren(Transform parent)
    {
        //This array will hold all children
        GameObject[] allChildren = new GameObject[parent.childCount];

        //Fill the array
        int childCount = 0;
        foreach (Transform child in parent.transform)
        {
            allChildren[childCount] = child.gameObject;
            childCount += 1;
        }

        return allChildren;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, dirtPileRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(dirtPileInterDistance, 0, 0));
    }
}
