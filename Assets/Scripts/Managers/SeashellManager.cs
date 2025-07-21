using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using QuantumTek.QuantumInventory;
using SerializableTypes;

public class SeashellManager : MonoBehaviour, IResetAtDawn
{

    public QI_ItemDatabase seashellDatabase;
    public Tilemap beachTiles;
    List<Vector3Int> beachPositions = new List<Vector3Int>();
    public int maxAmount = 20;
    float[,] noiseMap;
    [HideInInspector]
    public List<QI_ItemData> seashellItemList = new List<QI_ItemData>();
    public List<Vector3> seashellPositionList = new List<Vector3>();
    void Start()
    {
        //SetNoiseMap();

        BoundsInt bounds = beachTiles.cellBounds;
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int z = bounds.min.z; z < bounds.max.z; z++)
                {
                    var pos = new Vector3Int(x, y, z);
                    if (beachTiles.GetTile(pos) != null)
                        beachPositions.Add(pos);
                }
            }
        }

    }

    private void SetNoiseMap()
    {
        int seed = Random.Range(0, 10000);
        noiseMap = NoiseGenerator.GenerateNoiseMap(true, beachTiles.cellBounds.size.x, beachTiles.cellBounds.size.y, seed, 20, 4, 0.5f, 2, Vector2.zero);
    }

    void ClearAllShells()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
    }

    public void ResetAtDawn()
    {
        seashellItemList.Clear();
        seashellPositionList.Clear();
        ClearAllShells();
        SetNoiseMap();
        int amount = 0;
        foreach (var pos in beachPositions)
        {
            int x = (int)NumberFunctions.RemapNumber(pos.x, beachTiles.cellBounds.min.x, beachTiles.cellBounds.max.x, 0, beachTiles.cellBounds.size.x);
            int y = (int)NumberFunctions.RemapNumber(pos.y, beachTiles.cellBounds.min.y, beachTiles.cellBounds.max.y, 0, beachTiles.cellBounds.size.y);
            if (noiseMap[x, y] >= 0.88f)
            {
                var position = beachTiles.GetCellCenterWorld(pos);
                position.z++;
                Vector2 offset = Random.insideUnitCircle * 0.25f;
                position += (Vector3)offset;
                var hits = Physics2D.OverlapCircleAll(position, 0.03f, LayerMask.NameToLayer("Obstacle"), position.z);
                bool spawn = true;
                foreach(var hit in hits )
                {
                    if(hit.TryGetComponent(out DrawZasYDisplacement disp))
                        spawn = disp.positionZ <= 0;
                    if(!spawn) 
                        break;
                }
                if (spawn && amount < maxAmount)
                {
                    amount++;
                    var seashell = seashellDatabase.GetRandomWeightedItem();
                    seashellItemList.Add(seashell);
                    seashellPositionList.Add(position);
                    int r = Random.Range(0, seashell.ItemPrefabVariants.Count);
                    Instantiate(seashell.ItemPrefabVariants[r], position, Quaternion.identity, transform);
                }
                
            }
        }
    }

    public void SpawnFromSave(List<string> items, List<SVector3> positions)
    {
        seashellItemList.Clear();
        seashellPositionList.Clear();
        ClearAllShells();
        for (int i = 0; i < items.Count; i++)
        {
            var seashell = seashellDatabase.GetItem(items[i]);
            seashellItemList.Add(seashell);
            seashellPositionList.Add(positions[i]);
            int r = Random.Range(0, seashell.ItemPrefabVariants.Count);
            Instantiate(seashell.ItemPrefabVariants[r], positions[i], Quaternion.identity, transform);
        }
    }
}
