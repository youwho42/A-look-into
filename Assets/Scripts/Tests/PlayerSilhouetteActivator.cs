using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class PlayerSilhouetteActivator : MonoBehaviour
{

    

    PlayerInformation playerInformation;

    Transform playerTransform;
    public AnimationCurve treeSilhouetteCurveY;
    public AnimationCurve treeSilhouetteCurveX;
    PlayerSilhouetteManager silhouetteManager;
    GridManager gridManager;
    DrawZasYDisplacement displacement;
    bool lastBehindState;
    int gatherableLayer;
    bool potentialCliff;
    private void Start()
    {
        gatherableLayer = LayerMask.GetMask("Gatherable");
        silhouetteManager = PlayerSilhouetteManager.instance;
        playerTransform = transform;
        playerInformation = PlayerInformation.instance;
        gridManager = GridManager.instance;
        displacement = GetComponent<DrawZasYDisplacement>();
        GameEventManager.onPlayerPositionUpdateEvent.AddListener(CheckPotentialCliff);
    }
    private void OnDestroy()
    {
        GameEventManager.onPlayerPositionUpdateEvent.RemoveListener(CheckPotentialCliff);
    }
    private void Update()
    {
        if(potentialCliff)
            CheckForCliff();
        if (!silhouetteManager.isBehindCliff)
            CheckForTrees();
    }

    void CheckForTrees()
    {
        // Get all the gatherables
        var castPos = playerTransform.position/* + new Vector3(0, -0.3f, 0)*/;
        var hits = Physics2D.CircleCastAll(castPos, 0.5f, Vector2.down, 1.0f, gatherableLayer);
        float c = 0;
        foreach (var hit in hits)
        {
            // Is it a tree?
            if (hit.transform.gameObject.TryGetComponent(out TreeRustling tree))
            {

                
                if (hit.transform.position.y + tree.treeCollision.displacedPosition.y > playerTransform.position.y)
                {
                    
                    float diffY = playerTransform.position.y - hit.transform.position.y;
                    float normalDiffY = diffY / tree.treeCollision.displacedPosition.y;

                    float pineAngle = 1;
                    if (tree.isPineTree)
                        pineAngle = Mathf.Abs(normalDiffY - 1);
                    
                    float diffX = Mathf.Abs(playerTransform.position.x - hit.transform.position.x);
                    float normalDiffX = treeSilhouetteCurveX.Evaluate(NumberFunctions.RemapNumber(diffX, 0.0f, tree.treeCollision.size * pineAngle, 0.0f, 1.0f));
                    if (diffX > tree.treeCollision.size)
                        continue;
                    c = treeSilhouetteCurveY.Evaluate(normalDiffY * normalDiffX);
                    break;
                }
                    

            }
        }
        silhouetteManager.SetColor(c);
    }

    void CheckPotentialCliff(Vector3Int position)
    {
        Vector3 pos = playerTransform.position;

        potentialCliff = false;
        for (int i = 20; i > playerTransform.position.z + 1; i--)
        {
            pos.z = i;
            if (gridManager.GetTileExisting(pos))
            {
                potentialCliff = true;
                break;
            }
        }
    }

    private void CheckForCliff()
    {
        bool behindNow = false;

        Vector3 pos = playerTransform.position;
        pos.y += displacement.displacedPosition.y * 0.75f;

        for (int i = 20; i > playerTransform.position.z + 1; i--)
        {
            pos.z = i;
            if (gridManager.GetTileExisting(pos))
            {
                behindNow = true;
                break;
            }
        }
        if (behindNow != lastBehindState)
        {
            lastBehindState = behindNow;

            silhouetteManager.SetColor(behindNow ? 1 : 0, 0.25f, behindNow);
        }
    }


}
