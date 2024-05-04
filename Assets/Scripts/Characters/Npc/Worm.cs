using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{

    Animator animator;
    public GameObject worm;

    float emergeTime;
    float wiggleTime;
    public Vector2 minMaxBetweenEmerges;
    public Vector2 minMaxBetweenWiggles;
    bool isEmerged;
    Vector3 location;

    private void Start()
    {
        animator = worm.GetComponent<Animator>();
        
        isEmerged = false;
        ResetEmergeTime();
        worm.SetActive(false);
        
    }
    
    void ResetEmergeTime()
    {
        emergeTime = Random.Range(minMaxBetweenEmerges.x, minMaxBetweenEmerges.y);
        Invoke("AttemptEmerge", emergeTime);
    }
    private void ResetWiggleTime()
    {
        wiggleTime = Random.Range(minMaxBetweenWiggles.x, minMaxBetweenWiggles.y);
    }

    void AttemptEmerge()
    {
        ForceWormEmerge();
    }

    public void ForceWormEmerge()
    {
        if (!isEmerged)
        {
            ResetWiggleTime();
            location = ChooseEmergeLocation();
            StartCoroutine(StartWormAnimationCo());
        }
    }
    Vector3 ChooseEmergeLocation()
    {
        var groundMap = GridManager.instance.groundMap;
        Vector2 rand = Random.insideUnitCircle * 2f;
         
        Vector3 center = PlayerInformation.instance.player.position;
        location = center;
        var d = groundMap.WorldToCell(new Vector2(center.x + rand.x, center.y + rand.y));
        for (int z = groundMap.cellBounds.zMax; z > groundMap.cellBounds.zMin - 1; z--)
        {
            d.z = z;
            if (groundMap.GetTile(d) != null)
            {
                location = groundMap.GetCellCenterWorld(d);
                location += new Vector3(Random.Range(-.2f, .2f), Random.Range(-.2f, .2f), 1);
            }
        }
        return location;
    }
    
        
   
    IEnumerator StartWormAnimationCo()
    {
        worm.transform.position = location;
        isEmerged = true;
        worm.SetActive(true);
        yield return new WaitForSeconds(wiggleTime);
        animator.SetTrigger("Hide");
        yield return new WaitForSeconds(3f);
        worm.SetActive(false);
        ResetEmergeTime();
        isEmerged = false;
    }
  
   
}
