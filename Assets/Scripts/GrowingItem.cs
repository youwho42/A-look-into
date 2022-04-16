using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingItem : MonoBehaviour
{
    DayNightCycle dayNightCycle;
    [SerializeField]
    int daysToGrow;
    public int currentDay;
    public int currentTimeTick;
    bool canGrow;
    public float checkRadius;
    public List<GameObject> itemsToBecome = new List<GameObject>();
    SpriteRenderer mainSprite;
    public Sprite growSprite;
    public Sprite idleSprite;
    private void Start()
    {
        mainSprite = GetComponent<SpriteRenderer>();
        dayNightCycle = DayNightCycle.instance;

        CheckForNeighboringPlants();
        
        GameEventManager.onTimeTickEvent.AddListener(GetCurrentTime);
    }
    private void OnBecameVisible()
    {
        GameEventManager.onTimeTickEvent.AddListener(GetCurrentTime);
    }

    private void OnBecameInvisible()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(GetCurrentTime);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(GetCurrentTime);
    }

    public void SetCurrentDayTime(int day, int time)
    {
        currentDay = dayNightCycle.currentDayRaw;
        currentTimeTick = dayNightCycle.currentTimeRaw;
    }
    void GetCurrentTime(int tick)
    {
        CheckForNeighboringPlants();
        SetMainSprite();
        if (canGrow)
        {

            if(currentTimeTick == dayNightCycle.currentTimeRaw && dayNightCycle.currentDayRaw >=  currentDay + daysToGrow)
            {
                Grow();
            }

            

        }
            
        
    }

    
    
    void Grow()
    {
        int r = Random.Range(0, itemsToBecome.Count);
        var go = Instantiate(itemsToBecome[r], transform.position, Quaternion.identity);
        if(go.TryGetComponent(out SaveableItemEntity saveable))
        {
            saveable.GenerateId();
        }
        Destroy(gameObject);
    }

    public void CheckForNeighboringPlants()
    {
        
        canGrow = true;
        var hit = Physics2D.OverlapCircleAll(transform.position, checkRadius);
        if (hit.Length > 0)
        {
            
            foreach (var item in hit)
            {
                if (item.CompareTag("GrowingItem") && item.gameObject != this.gameObject)
                {
                    
                    canGrow = false;
                }
            }
        }
        
        
    }

    public void SetMainSprite()
    {

        mainSprite.sprite = canGrow ? growSprite : idleSprite;
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
