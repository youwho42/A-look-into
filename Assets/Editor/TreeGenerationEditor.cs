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
        treeGeneration.leavesShake.leafObjects.Clear();
        treeGeneration.shadows.subShadowSprites.Clear();
        allPositions.Clear();
        //create trunk
        int t = Random.Range(0, treeGeneration.trunks.Count);
        treeGeneration.trunkRenderer.sprite = treeGeneration.trunks[t].trunk;
        SetShadowPiece(treeGeneration.trunkRenderer.sprite, treeGeneration.trunkRenderer.transform.localPosition, treeGeneration.shadows.shadowTransform, false, false);
        //create rear and front leaves
        SetLeaves(treeGeneration.leavesRear, t, treeGeneration.lightestRear, treeGeneration.darkestRear, 0.5f);
        SetLeaves(treeGeneration.leavesFront, t, Color.white, treeGeneration.darkestFront);
        
    }
    

    void SetLeaves(Transform parent, int trunkIndex, Color min, Color max, float factor = 1.0f)
    {
        
        int a = Random.Range((int)(treeGeneration.trunks[trunkIndex].minPerLayer * factor), (int)(treeGeneration.trunks[trunkIndex].maxPerLayer * factor));
        for (int i = 0; i < a; i++)
        {
            var go = new GameObject();
            go.transform.parent = parent;

            var pos = GetPointInsideCollider(treeGeneration.trunks[trunkIndex].leafArea, 50);
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
            renderer.spriteSortPoint = SpriteSortPoint.Pivot;
            float ca = NumberFunctions.RemapNumber(z, 0.0f, 2.0f, 1.0f, 0.0f);
            var c = Color.Lerp(min, max, ca);
            renderer.color = c;
            treeGeneration.leavesShake.leafObjects.Add(go.transform);
            SetShadowPiece(renderer.sprite, go.transform.localPosition, treeGeneration.shadows.shadowTransform, renderer.flipX, true);

        }
    }

    void SetShadowPiece(Sprite sprite, Vector3 position, Transform parent, bool flipped, bool subSprite)
    {
        var go = new GameObject();
        go.transform.parent = parent;
        position.z *= 0.5f;
        go.transform.localPosition = position;
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.flipX = flipped;
        renderer.sprite = sprite;
        
        renderer.material = treeGeneration.shadowMaterial;
        if (!subSprite)
            treeGeneration.shadows.shadowSprite = renderer;
        else
            treeGeneration.shadows.subShadowSprites.Add(renderer);
        
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
        GameObject[] allShadows = GetAllChildren(treeGeneration.shadows.shadowTransform);

        //Now destroy them
        foreach (GameObject child in allRear)
        {
            DestroyImmediate(child);
        }
        foreach (GameObject child in allFront)
        {
            DestroyImmediate(child);
        }
        foreach (GameObject child in allShadows)
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
