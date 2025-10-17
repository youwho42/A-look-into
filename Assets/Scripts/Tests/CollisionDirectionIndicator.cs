using UnityEngine;
using System.Collections.Generic;
using Klaxon.GravitySystem;
using System.Linq;

public class CollisionDirectionIndicator : MonoBehaviour
{
    GravityItemNew gravityItem;
    CircleCollider2D circleCollider;
    float maxCollisionDist = 1.0f;
    List<GravityItemMovementFree> allFreeObjects = new List<GravityItemMovementFree>();
    bool canActivate;
    Transform playerTransform;
    private void Start()
    {
        playerTransform = transform;
        gravityItem = GetComponent<GravityItemNew>();
        circleCollider = GetComponent<CircleCollider2D>();
        //GameEventManager.onTimeTickEvent.AddListener(CheckForObjects);
    }
    public void AddFreeItemToList(GravityItemMovementFree freeObject)
    {
        
        if (!allFreeObjects.Contains(freeObject))
            allFreeObjects.Add(freeObject);
        
    }
    public void RemoveFreeItemFromList(GravityItemMovementFree freeObject)
    {
        if (allFreeObjects.Contains(freeObject))
            allFreeObjects.Remove(freeObject);
        
    }
    //private void OnDestroy()
    //{
    //    GameEventManager.onTimeTickEvent.RemoveListener(CheckForObjects);
    //}

    //void CheckForObjects(int tick)
    //{
    //    for (int i = 0; i < allFreeObjects.Count; i++)
    //    {
    //        if (NumberFunctions.GetDistanceV2(allFreeObjects[0].gameObject.transform.position, playerTransform.position) <= 1.0f)
    //        {
    //            canActivate = true;
    //            return;
    //        }
                
    //    }
    //    canActivate = false;
    //}

    void Update()
    {
        if (allFreeObjects.Count <= 0)
            return;
        //if (!canActivate)
        //    return;


        var hits = Physics2D.CircleCastAll((Vector2)playerTransform.position + circleCollider.offset, circleCollider.radius, gravityItem.currentDirection, maxCollisionDist, LayerMask.GetMask("Interactable"), playerTransform.position.z, playerTransform.position.z);
        
        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.gameObject.CompareTag("Ball") || hits[i].transform.gameObject.CompareTag("SculptureBall"))
                {
                    
                    var normies = -hits[i].normal;
                    if (hits[i].transform.TryGetComponent(out GravityItemMovementFree other))
                    {
                        if (other.velocity > 0.35f)
                        {
                            other.SetArrowInvisible(0.3f);
                            continue;
                        }

                        float angle = Mathf.Atan2(normies.y, normies.x) * Mathf.Rad2Deg;
                        other.directionArrow.rotation = Quaternion.Euler(0, 0, angle);
                        other.SetArrowVisible();
                        
                        break;
                    }
                }
            }
           
        }
        
    }


}
