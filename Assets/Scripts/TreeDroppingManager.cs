using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class TreeDroppingManager : MonoBehaviour
{

    public static TreeDroppingManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public int poolAmount;

    public TreeDropping treeDroppingPrefab;
    public ObjectPool<TreeDropping> treeDroppingPool;
    
    List<TreeDropping> startTreeDroppings = new List<TreeDropping>();


    public void Start()
    {

        treeDroppingPool = new ObjectPool<TreeDropping>
            (
                createFunc: CreateDropping,
                actionOnGet: GetFromPool,
                actionOnRelease: ReleaseToPool,
                defaultCapacity: poolAmount

            );

        
        //for (int i = 0; i < 6; i++)
        //{
        //    var t = treeDroppingPool.Get();
        //    t.TestDrop();
        //}
        
    }

    


    TreeDropping CreateDropping()
    {
        TreeDropping dropping = Instantiate(treeDroppingPrefab, transform);
        
        dropping.gameObject.SetActive(true);
        //dropping.SetSmell(this, currentZAsYDisplacement, smellData);
        return dropping;
    }

    void GetFromPool(TreeDropping dropping)
    {
        dropping.gameObject.SetActive(true);
    }

    void ReleaseToPool(TreeDropping dropping)
    {
        dropping.gameObject.SetActive(false);
    }
}
