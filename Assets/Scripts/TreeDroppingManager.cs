using UnityEngine;


public class TreeDroppingManager : MonoBehaviour
{

    public static TreeDroppingManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }


    ObjectPooler droppingsPool;
    public void Start()
    {
        droppingsPool = GetComponent<ObjectPooler>();
       
    }

    public TreeDropping GetDropping()
    {
        TreeDropping td = null;
        var go = droppingsPool.GetPooledObject();
        go.TryGetComponent(out td);
        return td;

    }


   
}
