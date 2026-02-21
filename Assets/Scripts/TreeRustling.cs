
using UnityEngine;

public class TreeRustling : MonoBehaviour, IWindEffect
{
    public SoundSet soundSet;
    AudioSource source;
    public DrawZasYDisplacement treeCollision;

    public Sprite treeDropppingSprite;
    public Vector2Int minMaxDroppings = new Vector2Int(1, 6);
    public float dropRadius;
    public TreeLeavesShake treeLeavesShake;
    public bool isPineTree;
    
    Vector3Int gridPosition;
    float minWindMagnitude = 2.0f;

    void Start()
    {
        source = GetComponentInParent<AudioSource>();
        gridPosition = GridManager.instance.GetTilePosition(gameObject.transform.position);
    }

    public void OnBecameVisible()
    {
        GameEventManager.onTimeTickEvent.AddListener(GetCurrentWind);
    }
    public void OnBecameInvisible()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(GetCurrentWind);   
    }

    void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(GetCurrentWind);
    }

    void GetCurrentWind(int tick)
    {
        if (Time.realtimeSinceStartup < 1)
            return;
        if (!CheckLocationIndex(tick))
            return;
        
        float m = WindManager.instance.GetWindMagnitude(gameObject.transform.position);

        bool chance = Random.value < m * 0.1f;
        if (m > minWindMagnitude && chance)
            Affect(true);
       
    }

    bool CheckLocationIndex(int tick)
    {
        int phase = gridPosition.x * 73856093 ^ gridPosition.y * 19349663;
        phase &= 0x7fffffff;

        int interval = 6;
        return ((tick + phase) % interval) == 0;

        
    }

    public void Affect(bool canDropItem)
    {
        if (!isActiveAndEnabled)
            return;
        int t = Random.Range(0, soundSet.clips.Length);
        soundSet.SetSource(source, t);
        source.volume = soundSet.volume;
        soundSet.Play();
        if(treeLeavesShake != null)
            treeLeavesShake.ShakeLeaves();
        if (canDropItem)
            DropDropping();
    }

    void DropDropping()
    {
        int r = Random.Range(minMaxDroppings.x, minMaxDroppings.y);
        for (int i = 0; i < r; i++)
        {
            var pos = Random.insideUnitCircle * dropRadius;
            var t = TreeDroppingManager.instance.GetDropping();
            t.SetDropping(transform.position + (Vector3)pos, treeCollision.positionZ, treeDropppingSprite);
        }

    }
    
}
