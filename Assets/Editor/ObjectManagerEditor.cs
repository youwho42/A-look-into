using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(ObjectManagerCircle))]
public class ObjectManagerEditor : Editor
{
    private ObjectManagerCircle objectManager;

    //The center of the circle
    private Vector3 center;
    int lastRandom;
    private void OnEnable()
    {
        objectManager = target as ObjectManagerCircle;

        //Hide the handles of the GO so we dont accidentally move it instead of moving the circle
        Tools.hidden = true;
    }

    private void OnDisable()
    {
        //Unhide the handles of the GO
        Tools.hidden = false;
    }

    
    private void OnSceneGUI()
    {
        if (GridManager.instance.groundMap == null)
            return;
        //Move the circle when moving the mouse
        //A ray from the mouse position
        Vector3 mousePosition = Event.current.mousePosition;
        mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
        mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
        //var hit = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        //var hit = Camera.current.ScreenPointToRay(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin);
        //var position = hit.origin;

        mousePosition.z = 0;

        
        int offsetZ = objectManager.GetTileZ(mousePosition);
        
       

        

            //Where did we hit the ground?
        center = mousePosition;

        //Need to tell Unity that we have moved the circle or the circle may be displayed at the old position
        SceneView.RepaintAll();

        

        //Display the circles
        Handles.color = Color.blue;

        Handles.DrawWireDisc(center, Vector3.forward, objectManager.radius);

        Handles.color = Color.white;

        Handles.DrawWireDisc(center + new Vector3(0, offsetZ * GlobalSettings.SpriteDisplacementY, offsetZ), Vector3.forward, objectManager.radius);


        //Add or remove objects with left mouse click

        //First make sure we cant select another gameobject in the scene when we click
        HandleUtility.AddDefaultControl(0);

        //Have we clicked with the left mouse button?
        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            //Should we add or remove objects?
            if (objectManager.action == ObjectManagerCircle.Actions.AddObjects)
            {
                AddNewPrefabs(center + new Vector3(0, offsetZ * GlobalSettings.SpriteDisplacementY, offsetZ));

                
            }
            else if (objectManager.action == ObjectManagerCircle.Actions.RemoveObjects)
            {
                objectManager.RemoveObjects(center);

            }
            
            MarkSceneAsDirty();
        }
    }

    //Add buttons this scripts inspector
    public override void OnInspectorGUI()
    {
        //Add the default stuff
        DrawDefaultInspector();

        //Remove all objects when pressing a button
        if (GUILayout.Button("Remove all objects"))
        {
            //Pop-up so you don't accidentally remove all objects
            if (EditorUtility.DisplayDialog("Safety check!", "Do you want to remove all objects?", "Yes", "No"))
            {
                objectManager.RemoveAllObjects();

                MarkSceneAsDirty();
            }
        }
    }

    //Force unity to save changes or Unity may not save when we have instantiated/removed prefabs despite pressing save button
    private void MarkSceneAsDirty()
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
    }

    //Instantiate prefabs at random positions within the circle
    private void AddNewPrefabs(Vector3 center)
    {
        if (!objectManager.usePoissonDisc)
        {
            //How many prefabs do we want to add
            int howManyObjects = objectManager.howManyObjects;

            //Which prefab do we want to add
            for (int i = 0; i < howManyObjects; i++)
            {
                
                GameObject newGO = objectManager.useNeighboringObjects? CreateGameObjectFromNeighbor(center) : CreateGameObjectFromPrefab();
                //Send it to the main script to add it at a random position within the circle
                objectManager.AddPrefab(newGO, center);
            }
        }
        else
        {
            Vector2 area = new Vector2(objectManager.radius*2, objectManager.radius*2);

            List<Vector2> positions = new List<Vector2>();
            positions = PoissonDiscSampler.GeneratePoints(objectManager.poissonDiscMinRadius, area);
            for (int i = 0; i < positions.Count; i++)
            {
                //Check if position is in the circle
                if (Vector2.Distance(positions[i], area/2) < objectManager.radius)
                {
                    GameObject newGO = objectManager.useNeighboringObjects ? CreateGameObjectFromNeighbor(center) : CreateGameObjectFromPrefab();
                    //Send it to the main script to add it at a random position within the circle
                    objectManager.AddPrefab(newGO, center, positions[i]);
                }
                
            }
        }
        
    }
    GameObject CreateGameObjectFromPrefab()
    {
        
        int rand = -1;
        if (objectManager.prefabGO.Length > 1)
        {
            do
            {
                rand = Random.Range(0, objectManager.prefabGO.Length);

            }
            while (lastRandom == rand);
            lastRandom = rand;
        }
        else 
            rand = 0;
            
        GameObject prefabGO = objectManager.prefabGO[rand];
        GameObject newGO = PrefabUtility.InstantiatePrefab(prefabGO) as GameObject;
        if(objectManager.canFlipX)
        {
            int f = Random.Range(0, 2);
            if (newGO.TryGetComponent(out SpriteRenderer sprite))
                sprite.flipX = f == 0 ? false : true;
        }
        return newGO;


    }

    GameObject CreateGameObjectFromNeighbor(Vector3 center)
    {
        

        // get closest neighbor object type (if closest = null set to none)
        List<NeighborObjectType> closest = new List<NeighborObjectType>();
        List<NeighboringObject> allNeighbors = new List<NeighboringObject>();
        var hit = Physics2D.OverlapCircleAll(center, 1.5f);
        foreach(var neighbor in hit )
        {
            if (Mathf.Abs(neighbor.transform.position.z - center.z) > 1)
                continue;
            if(neighbor.TryGetComponent(out NeighboringObject n))
                allNeighbors.Add(n);
        }
        
        allNeighbors = allNeighbors.OrderBy(x => Vector3.Distance(center, x.transform.position)).Take(3).ToList();
        foreach (var item in allNeighbors)
        {
            closest.Add(item.NeighborObjectType);
        }
        while (closest.Count < 3)
        {
            closest.Add(NeighborObjectType.None);
        }

        
        // make a list of all the items that could spawn near type
        List<GameObject> possibleItems = new List<GameObject>();
        
       
        foreach (var flower in objectManager.prefabGO)
        {
            foreach (var neighbor in closest)
            {
                if (flower.TryGetComponent(out NeighboringObjectsSpawn spawn))
                {
                    foreach (var type in spawn.neighboringObjects)
                    {
                        if (type == neighbor)
                            possibleItems.Add(flower);
                    }
                }
            }
            
        }



        // choose and return an item from said list

        int rand = -1;
        if (possibleItems.Count > 1)
        {
            do
            {
                rand = Random.Range(0, possibleItems.Count);

            }
            while (lastRandom == rand);
            lastRandom = rand;
        }
        else
            rand = 0;

        GameObject prefabGO = possibleItems.Count == 0 ? objectManager.prefabGO[Random.Range(0, objectManager.prefabGO.Length)] : possibleItems[rand];
        GameObject newGO = PrefabUtility.InstantiatePrefab(prefabGO) as GameObject;
        if (objectManager.canFlipX)
        {
            int f = Random.Range(0, 2);
            if (newGO.TryGetComponent(out SpriteRenderer sprite))
                sprite.flipX = f == 0 ? false : true;
        }
        return newGO;
    }
}