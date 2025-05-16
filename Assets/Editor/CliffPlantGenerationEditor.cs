using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateCliffPlant))]
[CanEditMultipleObjects]
public class CliffPlantGenerationEditor : Editor
{
    private GenerateCliffPlant cliffPlantGeneration;
    List<Vector2> allPositions = new List<Vector2>();

    private void OnEnable()
    {
        cliffPlantGeneration = target as GenerateCliffPlant;
    }

    public override void OnInspectorGUI()
    {
        //Add the default stuff
        DrawDefaultInspector();

        //Generate tree
        if (GUILayout.Button("Generate Plant"))
        {
            RemoveAllObjects(cliffPlantGeneration.plantHolder);
            ToggleColliderEnabled(true);
            GeneratePlantObjects();
            ToggleColliderEnabled(false);
            MarkSceneAsDirty();
        }
        if (GUILayout.Button("Generate Vine"))
        {
            RemoveAllObjects(cliffPlantGeneration.vineHolder);
            RemoveAllObjects(cliffPlantGeneration.plantHolder);
            GenerateVineObjects();
            MarkSceneAsDirty();
        }

    }

    private void MarkSceneAsDirty()
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
    }

    void GenerateVineObjects()
    {
        var maxHeight = cliffPlantGeneration.displacement - Random.Range(0.0f, cliffPlantGeneration.vineObject.heightVariance);
        CreateVine(null, VineSegmentName.Base, maxHeight);
    }




    void CreateVine(GenerateVineObject lastVineObject, VineSegmentName segmentType, float maxHeight)
    {
        List<GenerateVineObject> objectPool = new List<GenerateVineObject>();
        switch (segmentType)
        {
            case VineSegmentName.Base:
                objectPool = cliffPlantGeneration.vineObject.vineBases;
                break;
            case VineSegmentName.Mid:
                objectPool = cliffPlantGeneration.vineObject.vineSegments;
                break;
            case VineSegmentName.Tip:
                objectPool = cliffPlantGeneration.vineObject.vineTips;
                break;

        }

        int r = Random.Range(0, objectPool.Count);

        GenerateVineObject newGO = PrefabUtility.InstantiatePrefab(objectPool[r]) as GenerateVineObject;
        newGO.transform.parent = cliffPlantGeneration.vineHolder;

        if(lastVineObject == null)
            newGO.transform.localPosition = Vector3.zero;
        else
            newGO.transform.position = lastVineObject.endPoint.position;

        float z = newGO.transform.localPosition.y / GlobalSettings.SpriteDisplacementY;
        newGO.transform.localPosition = new Vector3(newGO.transform.localPosition.x, newGO.transform.localPosition.y, z + 0.001f);
        if (cliffPlantGeneration.vineObject.flipped)
            newGO.transform.localScale = new Vector3(-1, 1, 1);

        Color c = cliffPlantGeneration.vineObject.flipped ? cliffPlantGeneration.vineObject.darkest : cliffPlantGeneration.vineObject.lightest;
        newGO.GetComponentInChildren<SpriteRenderer>().color = c;

        if (newGO.transform.localPosition.z < maxHeight)
        {
            float nextZ = newGO.transform.localPosition.z + (newGO.endPoint.localPosition.y / GlobalSettings.SpriteDisplacementY);
            VineSegmentName segmentName = nextZ < maxHeight ? VineSegmentName.Mid : VineSegmentName.Tip;
            CreateVine(newGO, segmentName, maxHeight);
        }

    }

    void GeneratePlantObjects()
    {
        allPositions.Clear();
        SetLeaves();
    }

    void SetLeaves()
    {
        
        foreach (var leafObject in cliffPlantGeneration.leaveObjects)
        {
            if (!leafObject.isActive)
                continue;

            
            int a = Random.Range(leafObject.minAmount, leafObject.maxAmount);
            for (int i = 0; i < a; i++)
            {
                var pos = GetPointInsideCollider(leafObject.leafArea, 50);
                if (pos == -Vector2.one)
                    continue;

                var go = new GameObject();
                go.transform.parent = cliffPlantGeneration.plantHolder;

                
                allPositions.Add(pos);
                go.transform.position = pos;
                float z = go.transform.localPosition.y / GlobalSettings.SpriteDisplacementY;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, z + 0.001f);

                int s = Random.Range(0, cliffPlantGeneration.leaves.Count);
                var renderer = go.AddComponent<SpriteRenderer>();
                renderer.flipX = leafObject.flipped;
                renderer.sprite = cliffPlantGeneration.leaves[s];
                renderer.spriteSortPoint = SpriteSortPoint.Pivot;
                var offset = Mathf.Abs(go.transform.localPosition.x) / z * z;
                
                Color c = leafObject.darkest;
                if (cliffPlantGeneration.isOnCliffEdge)
                {
                    var t = NumberFunctions.RemapNumber(z + offset, 0.0f, cliffPlantGeneration.displacement, 1, 0);
                    var ease = cliffPlantGeneration.easeInAmount.Evaluate(t);
                    c = Color.Lerp(leafObject.lightest, leafObject.darkest, ease);
                }
                
                renderer.color = c;
            }
            
        }
        
    }

    void ToggleColliderEnabled(bool state)
    {
        foreach (var item in cliffPlantGeneration.leaveObjects)
        {
            var allColliders = item.leafArea.GetComponentsInChildren<Collider2D>();
            foreach (var collider in allColliders)
            {
                collider.enabled = state;
            }
            //item.leafArea.enabled = state;
        }
    }
    public Vector2 GetPointInsideCollider(Collider2D collider, int remainingTries)
    {
        if (remainingTries == 0)
            return -Vector2.one;
        Vector2 point;
        point.x = Random.Range(collider.bounds.min.x, collider.bounds.max.x);
        point.y = Random.Range(collider.bounds.min.y, collider.bounds.max.y);

        bool tooClose = false;
        foreach (var pos in allPositions)
        {
            var dist = (point - pos).sqrMagnitude;
            if (dist < cliffPlantGeneration.minDistance * cliffPlantGeneration.minDistance)
                tooClose = true;
        }

        point = collider.OverlapPoint(point) && !tooClose ? point : GetPointInsideCollider(collider, remainingTries - 1);
        return point;
    }

    public void RemoveAllObjects(Transform parent)
    {
        //Get an array with all children to this transform
        GameObject[] allLeaves = GetAllChildren(parent);
        

        //Now destroy them
        foreach (GameObject child in allLeaves)
        {
            DestroyImmediate(child);
        }
        
    }
    private GameObject[] GetAllChildren(Transform parent)
    {
        //This array will hold all children
        GameObject[] allChildren = new GameObject[parent.childCount];

        //Fill the array
        int childCount = 0;
        foreach (Transform child in parent.transform)
        {
            allChildren[childCount] = child.gameObject;
            childCount += 1;
        }

        return allChildren;
    }
}
