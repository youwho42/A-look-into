using System.Collections.Generic;
using UnityEngine;
using Klaxon.StatSystem;
using System.Collections;

public class FlumpOozeContainer : MonoBehaviour
{
    public FlumpOoze flumpOoze;
    List<Vector3Int> tilePositions = new List<Vector3Int>();
    List<FlumpOoze> oozeSprites = new List<FlumpOoze>();
    public Color cleanOozeColorA;
    public Color cleanOozeColorB;
    public Transform oozeContainer;
    public ReplaceObjectOnItemDrop replaceObject;


    public void SetOozeFromSave(Color color, Vector3 position)
    {
        var ooze = Instantiate(flumpOoze, position, Quaternion.identity);
        ooze.SetOoze(oozeContainer, color, true);
    }

    public void SetOccupiedTile(Vector3 position, FlumpOoze ooze)
    {
        var pos = GridManager.instance.GetTilePosition(position);
        oozeSprites.Add(ooze);
        if (!tilePositions.Contains(pos))
        {
            if (PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup.ContainsKey(pos))
            {
                tilePositions.Add(pos);
                
                PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup[pos].movementPenaltyModifier += 4000;
            }
                
        }
        //replaceObject.ShowObjects(true);
        replaceObject.CheckForObjects();
    }

    public void ResetOccupiedTiles()
    {
        for (int i = 0; i < tilePositions.Count; i++)
        {
            if (!PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup.ContainsKey(tilePositions[i]))
                return;

            PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup[tilePositions[i]].movementPenaltyModifier -= 4000;
            
        }
        
    }

    public void StartCleanOoze()
    {
        for (int i = 0; i < oozeSprites.Count; i++)
        {
            StartCoroutine(CleanOozeSpritesCo(oozeSprites[i]));
        }
    }


    IEnumerator CleanOozeSpritesCo(FlumpOoze ooze)
    {
        yield return new WaitForSeconds(1.3f);
        var cleanColor = Color.Lerp(cleanOozeColorA, cleanOozeColorB, Random.value);
        var startColor = ooze.oozeSprite.color;
        float timer = 0;
        while (timer < 3)
        {
            timer += Time.deltaTime;
            Color c = Color.Lerp(startColor, cleanColor, timer / 3);
            float a = Mathf.Lerp(1, 0, timer / 3);
            c.a = a;
            ooze.oozeSprite.color = c;
            ooze.stinkTrail.color = c;
            yield return null;
        }
        Destroy(ooze.gameObject);
        yield return null;
        
        ResetOccupiedTiles();
        yield return null;
        DestroyOoze();
    }
    void DestroyOoze()
    {
        Destroy(gameObject);
    }
}
