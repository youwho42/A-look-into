using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceObjectOnItemDrop : MonoBehaviour
{
    public float checkRadius;
    public Vector2 checkOffset;
    public List<GameObject> grassObjects = new List<GameObject>();
    private void Start()
    {
        CheckForObjects();
    }
    public void CheckForObjects()
    {
        var hit = Physics2D.OverlapCircleAll((Vector2)transform.position + checkOffset, checkRadius);
        if(hit != null)
        {
            foreach (var item in hit)
            {
                if (!item.CompareTag("Grass"))
                    continue;

                grassObjects.Add(item.gameObject);
            }
            HideObjects();
        }
    }

    void HideObjects()
    {
        if(grassObjects.Count > 0)
        {
            foreach (var item in grassObjects)
            {
                item.SetActive(false);
            }
        }
    }
    public void ShowObjects()
    {
        if (grassObjects.Count > 0)
        {
            foreach (var item in grassObjects)
            {
                item.SetActive(true);
            }
        }
        grassObjects.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + checkOffset, checkRadius);
    }

}
