using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(TreeGeneration))]
public class TreeGenerationEditor : Editor
{
    private TreeGeneration treeGeneration;
    List<Vector2> allPositions = new List<Vector2>();

    private void OnEnable()
    {
        treeGeneration = target as TreeGeneration;
    }

    public override void OnInspectorGUI()
    {
        //Add the default stuff
        DrawDefaultInspector();

        //Generate tree
        if (GUILayout.Button("Generate Tree"))
        {
            RemoveAllObjects();
            GenerateTreeObjects();

            MarkSceneAsDirty();
            
        }
        
    }

    private void MarkSceneAsDirty()
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
    }

    void GenerateTreeObjects()
    {
        allPositions.Clear();
        //create trunk
        int t = Random.Range(0, treeGeneration.trunks.Count);
        treeGeneration.trunkRenderer.sprite = treeGeneration.trunks[t];
        //create rear and front leaves
        SetLeaves(treeGeneration.leavesRear, treeGeneration.darkestFront, treeGeneration.darkestRear, 2);
        SetLeaves(treeGeneration.leavesFront, Color.white, treeGeneration.darkestFront);
        
        
    }

    void SetLeaves(Transform parent, Color min, Color max, int factor = 1)
    {
        int a = Random.Range(treeGeneration.minPerLayer/factor, treeGeneration.maxPerLayer/factor);
        for (int i = 0; i < a; i++)
        {
            var go = new GameObject();
            go.transform.parent = parent;

            var pos = GetPointInsideCollider(treeGeneration.leafArea, 50);
            if (pos == -Vector2.one)
                continue;
            allPositions.Add(pos);
            go.transform.position = pos;
            float z = go.transform.localPosition.y / GlobalSettings.SpriteDisplacementY;
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, z + 0.01f);

            int s = Random.Range(0, treeGeneration.leaves.Count);
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.flipX = Random.value < 0.5f;
            renderer.sprite = treeGeneration.leaves[s];
            var c = Color.Lerp(min, max, Random.value);
            renderer.color = c;
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
            if(dist < treeGeneration.minDistance * treeGeneration.minDistance)
                tooClose = true;
        }

        point = collider.OverlapPoint(point) && !tooClose ? point : GetPointInsideCollider(collider, remainingTries - 1);
        return point;
    }

    

    //Remove all objects
    public void RemoveAllObjects()
    {
        //Get an array with all children to this transform
        GameObject[] allRear = GetAllChildren(treeGeneration.leavesRear.transform);
        GameObject[] allFront = GetAllChildren(treeGeneration.leavesFront.transform);

        //Now destroy them
        foreach (GameObject child in allRear)
        {
            DestroyImmediate(child);
        }
        foreach (GameObject child in allFront)
        {
            DestroyImmediate(child);
        }
    }

    //Get an array with all children to this GO
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
