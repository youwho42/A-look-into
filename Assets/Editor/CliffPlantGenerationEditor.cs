using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateCliffPlant))]
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
            RemoveAllObjects();
            GeneratePlantObjects();

            MarkSceneAsDirty();

        }

    }

    private void MarkSceneAsDirty()
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
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
                var go = new GameObject();
                go.transform.parent = cliffPlantGeneration.plantHolder;

                var pos = GetPointInsideCollider(leafObject.leafArea, 50);
                if (pos == -Vector2.one)
                    continue;
                allPositions.Add(pos);
                go.transform.position = pos;
                float z = go.transform.localPosition.y / GlobalSettings.SpriteDisplacementY;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, z + 0.01f);

                int s = Random.Range(0, cliffPlantGeneration.leaves.Count);
                var renderer = go.AddComponent<SpriteRenderer>();
                renderer.flipX = leafObject.flipped;
                renderer.sprite = cliffPlantGeneration.leaves[s];
                renderer.spriteSortPoint = SpriteSortPoint.Pivot;
                var offset = Mathf.Abs(go.transform.localPosition.x) / z * z;
                
                Color c = leafObject.darkest;
                if (cliffPlantGeneration.isOnCliffEdge)
                {
                    var t = NumberFunctions.RemapNumber(z + offset, 0.0f, cliffPlantGeneration.displacement.positionZ, 1, 0);
                    var ease = cliffPlantGeneration.easeInAmount.Evaluate(t);
                    c = Color.Lerp(leafObject.lightest, leafObject.darkest, ease);
                }
                
                renderer.color = c;
            }
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

    public void RemoveAllObjects()
    {
        //Get an array with all children to this transform
        GameObject[] allLeaves = GetAllChildren(cliffPlantGeneration.plantHolder);
        

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
