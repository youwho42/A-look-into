using UnityEngine;
using System.Collections.Generic;
using Klaxon.SaveSystem;



public class SpadeDigAreaManager : MonoBehaviour, IResetAtDawn
{
    GridManager grid;
    public SpadeAreaInteractor digArea;
    GameObject holder;
   
    void Start()
    {
        grid = GridManager.instance;
        //holder = new GameObject();
    }

    public void ResetAtDawn()
    {
        if (holder == null)
            holder = new GameObject();
        DestroyAllObjects();

        int amount = 0;
        int range = Random.Range(40, 60);
        
        while (amount < range)
        {
            if (grid.GetRandomTile(out Vector3 pos))
            {
                var hits = Physics2D.OverlapCircleAll(pos, .2f, LayerMask.GetMask("Obstacle"), pos.z, pos.z);

                if (hits.Length > 0)
                    continue;
                
                var currentDigArea = Instantiate(digArea, pos, Quaternion.identity, holder.transform);

                if(currentDigArea.TryGetComponent(out SaveableItemEntity saveable))
                    saveable.GenerateId();
                
                amount++;
            }
        }
    }


    void DestroyAllObjects()
    {
        
        foreach (Transform child in holder.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
