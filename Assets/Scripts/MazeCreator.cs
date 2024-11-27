using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Klaxon.Interactable;
using Klaxon.SaveSystem;
using System;
using System.Linq;

namespace Klaxon.MazeTech
{
    public class MazeTile
    {
        public GameObject[] NW;
        public GameObject[] NE;
        public GameObject[] SE;
        public GameObject[] SW;
        public bool IsCollapsed;
        public int Index;
        public bool HasNeighbor;
        public bool EndTile;
        public Vector3 TileCenter;
        public MazeTile(GameObject[] NW, GameObject[] NE, GameObject[] SE, GameObject[] SW, int index)
        {
            this.NW = NW;
            this.NE = NE;
            this.SE = SE;
            this.SW = SW;
            this.Index = index;
        }
    }


    public class MazeCreator : MonoBehaviour
    {
        enum MazeDirections
        {
            NW,
            NE,
            SE,
            SW
        }
        
        public IsometricGridObject gridObject;

        public List<GameObject> itemsToPlace = new List<GameObject>();
        public List<MazePost> endPostItems = new List<MazePost>();
        public int zLevel;
        public int mazeSize = 37;
        int tileSize = 3;
        public int rows = 12;
        public Vector2Int basePosition;
        [HideInInspector]
        public Vector3 convertedBasePosition;
        public Transform hedgeHolder;
        public Transform itemsHolder;
        public QI_ItemDatabase mazeItems;

        [HideInInspector]
        public GameObject[,] hedgePieces;
        MazeStringGame mazeString;
        [HideInInspector]
        public List<MazeTile> allTiles = new List<MazeTile>();
        List<MazeTile> nonCollapsedTiles = new List<MazeTile>();
        List<MazeTile> collapsedTiles = new List<MazeTile>();
        public bool mazeSet;
        public MazeDoor mazeDoor;

        [HideInInspector]
        public bool initialized;

        private void Start()
        {
            mazeString = GetComponent<MazeStringGame>();
            EmptyHolder(hedgeHolder);
            EmptyHolder(itemsHolder);
            if(!initialized)
                SetGridAndTiles();
        }

        
        public void SetGridAndTiles()
        {
            initialized = true;
            allTiles.Clear();
            hedgePieces = new GameObject[mazeSize, mazeSize];
            convertedBasePosition = gridObject.GetWorldPosition(basePosition.x, basePosition.y);
            for (int x = 0; x < mazeSize; x++)
            {
                for (int y = 0; y < mazeSize; y++)
                {
                    GameObject go = null;
                    if (x == 0 || x % tileSize == 0 || y == 0 || y % tileSize == 0)
                        go = PlaceObject(gridObject.GetWorldPosition(x, y), y + x * mazeSize, hedgeHolder);

                    hedgePieces[x, y] = go;
                }
            }
            int index = 0;
            for (int x = 0; x < mazeSize - 1; x += tileSize)
            {
                for (int y = 0; y < mazeSize - 1; y += tileSize)
                {

                    GameObject[] NE = new GameObject[2];
                    NE[0] = hedgePieces[x + 1, y + 3];
                    NE[1] = hedgePieces[x + 2, y + 3];

                    GameObject[] NW = new GameObject[2];
                    NW[0] = hedgePieces[x + 3, y + 1];
                    NW[1] = hedgePieces[x + 3, y + 2];

                    GameObject[] SE = new GameObject[2];
                    SE[0] = hedgePieces[x + 1, y];
                    SE[1] = hedgePieces[x + 2, y];

                    GameObject[] SW = new GameObject[2];
                    SW[0] = hedgePieces[x, y + 1];
                    SW[1] = hedgePieces[x, y + 2];

                    var midA = hedgePieces[x, y];
                    var midB = hedgePieces[x + 3, y + 3];


                    MazeTile tileObject = new MazeTile(NE, NW, SE, SW, index);

                    tileObject.TileCenter = Vector3.Lerp(midA.transform.position, midB.transform.position, 0.5f);
                    tileObject.HasNeighbor = false;
                    tileObject.IsCollapsed = false;
                    allTiles.Add(tileObject);

                    index++;
                }
            }
            for (int i = 0; i < endPostItems.Count; i++)
            {
                endPostItems[i].gameObject.SetActive(false);
            }
            SetEntrance();
            mazeSet = false;
        }

        void EmptyHolder(Transform holder)
        {
            foreach (Transform child in holder)
            {
                Destroy(child.gameObject);
            }
        }
        
        public void SetMazeFromSave(bool[,] states, List<Vector3> postPositions, bool mazeState)
        {
            if (!initialized)
                SetGridAndTiles();


            // Setting Hedges
            for (int x = 0; x < mazeSize; x++)
            {
                for (int y = 0; y < mazeSize; y++)
                {
                    if(hedgePieces[x, y] != null)
                        hedgePieces[x, y].SetActive(states[x, y]);
                }
            }

            // Setting Posts
            for (int i = 0; i < endPostItems.Count; i++)
            {
                endPostItems[i].transform.position = postPositions[i];
                endPostItems[i].SetPostSign(i);
                endPostItems[i].gameObject.SetActive(true);
            }

            // Maze State
            mazeSet = mazeState;

        }

        void SetEntrance()
        {
            
            foreach (var item in allTiles[rows * (rows-1)].NE)
            {
                item.SetActive(false);
            }
            
        }

        
        public void ResetMaze()
        {
            EmptyHolder(itemsHolder);
            mazeString.ResetLine();
            collapsedTiles.Clear();
            nonCollapsedTiles.Clear();
            mazeSet = false;
            mazeDoor.ResetDoor();
            foreach (var tile in allTiles)
            {
                foreach (var item in tile.NW)
                    item.SetActive(true);
                
                foreach (var item in tile.NE)
                    item.SetActive(true);
                
                foreach (var item in tile.SE)
                    item.SetActive(true);
                
                foreach (var item in tile.SW)
                    item.SetActive(true);
                
                tile.IsCollapsed = false;
                tile.HasNeighbor = false;
                tile.EndTile = false;
            }
            for (int i = 0; i < endPostItems.Count; i++)
            {
                endPostItems[i].gameObject.SetActive(false);
            }
            SetEntrance();
        }


        
        public void StartMazeCreation()
        {
            ChooseRandomStartTile();
            
        }

        void ChooseRandomStartTile()
        {
            SetNonCollapsedTiles();
            if (nonCollapsedTiles.Count <= 0)
                return;

            int r = UnityEngine.Random.Range(0, nonCollapsedTiles.Count);
            SetCorridor(nonCollapsedTiles[r].Index);
            SetEntrance();
            
        }


        void SetNonCollapsedTiles()
        {
            nonCollapsedTiles.Clear();
            for (int i = 0; i < allTiles.Count; i++)
            {
                if (!allTiles[i].IsCollapsed)
                    nonCollapsedTiles.Add(allTiles[i]);
            }
        }


        void ChooseRandomCollapsedTile()
        {

            SetCollapsedTiles();

            if (collapsedTiles.Count <= 0)
            {
                SetItems();
                SetPosts();
                mazeSet = true;
                return;
            }
                


            int r = UnityEngine.Random.Range(0, collapsedTiles.Count);
            SetCorridor(collapsedTiles[r].Index);
        }

        void SetItems()
        {
            List<MazeTile> mazeTiles= new List<MazeTile>();
            foreach (var tile in allTiles)
            {
                if (!tile.EndTile)
                    mazeTiles.Add(tile);
            }

            foreach (var tile in mazeTiles)
            {
                float r = UnityEngine.Random.Range(0.0f, 1.0f);
                if (r < 0.5f)
                    continue;
                var item = mazeItems.GetRandomWeightedItem();
                var go = Instantiate(item.ItemPrefabVariants[0], tile.TileCenter, Quaternion.identity, itemsHolder);
                if(go.TryGetComponent(out InteractablePickUp i))
                {
                    float height = 0.6f;
                    i.visualItem.transform.localPosition = new Vector3(0, GlobalSettings.SpriteDisplacementY * height, height);
                    i.pickupQuantity = UnityEngine.Random.Range(1, 6);
                }
                if (go.TryGetComponent(out SaveableItemEntity saveable))
                {
                    saveable.GenerateId();
                }
            }
        }


        void SetPosts()
        {
            List<MazeTile> endTiles = new List<MazeTile>();
            foreach (var tile in allTiles)
            {
                
                if (tile.EndTile && tile.Index != rows * (rows - 1))
                    endTiles.Add(tile);
            }
            if (endTiles.Count <= 0)
                return;

           
            var rand = new System.Random();
            var t =  endTiles.OrderBy(a => rand.Next()).ToList();
            


            while (t.Count > 3)
            {
                int r = UnityEngine.Random.Range(0, t.Count);
                t.RemoveAt(r);
            }
            for (int i = 0; i < t.Count; i++)
            {
                endPostItems[i].transform.position = t[i].TileCenter;
                endPostItems[i].SetPostSign(i);
                endPostItems[i].gameObject.SetActive(true);
            }
            
        }


        void SetCollapsedTiles()
        {
            collapsedTiles.Clear();

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < rows; y++)
                {

                    int ind = y + x * rows;
                    if (!allTiles[ind].IsCollapsed)
                        continue;

                    int neighbor;
                    allTiles[ind].HasNeighbor = false;
                    if (y < rows - 1)
                    {
                        neighbor = (y + 1) + (x * rows);
                        if (!allTiles[neighbor].IsCollapsed)
                        {
                            allTiles[ind].HasNeighbor = true;
                        }

                    }


                    if (x < rows - 1)
                    {
                        neighbor = y + ((x + 1) * rows);
                        if (!allTiles[neighbor].IsCollapsed)
                        {
                            allTiles[ind].HasNeighbor = true;
                        }

                    }


                    if (x > 0)
                    {
                        neighbor = y + ((x - 1) * rows);
                        if (!allTiles[neighbor].IsCollapsed)
                        {
                            allTiles[ind].HasNeighbor = true;
                        }

                    }


                    if (y > 0)
                    {
                        neighbor = (y - 1) + (x * rows);
                        if (!allTiles[neighbor].IsCollapsed)
                        {
                            allTiles[ind].HasNeighbor = true;
                        }

                    }
                    if (allTiles[ind].HasNeighbor)
                        collapsedTiles.Add(allTiles[ind]);

                }
            }
        }

        void SetCorridor(int allIndex)
        {
            if (allTiles[allIndex].IsCollapsed && !allTiles[allIndex].HasNeighbor)
            {
                return;
            }

            List<MazeDirections> possibleDirections = new List<MazeDirections>();
            List<int> possibleNeighbors = new List<int>();
            allTiles[allIndex].HasNeighbor = false;
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    int ind = y + x * rows;
                    if (ind == allIndex)
                    {

                        int neighbor;

                        if (y < rows - 1)
                        {
                            neighbor = (y + 1) + (x * rows);
                            if (!allTiles[neighbor].IsCollapsed)
                            {
                                possibleDirections.Add(MazeDirections.NW);
                                possibleNeighbors.Add(neighbor);
                            }

                        }


                        if (x < rows - 1)
                        {
                            neighbor = y + ((x + 1) * rows);
                            if (!allTiles[neighbor].IsCollapsed)
                            {
                                possibleDirections.Add(MazeDirections.NE);
                                possibleNeighbors.Add(neighbor);
                            }

                        }


                        if (x > 0)
                        {
                            neighbor = y + ((x - 1) * rows);
                            if (!allTiles[neighbor].IsCollapsed)
                            {
                                possibleDirections.Add(MazeDirections.SW);
                                possibleNeighbors.Add(neighbor);
                            }

                        }


                        if (y > 0)
                        {
                            neighbor = (y - 1) + (x * rows);
                            if (!allTiles[neighbor].IsCollapsed)
                            {
                                possibleDirections.Add(MazeDirections.SE);
                                possibleNeighbors.Add(neighbor);
                            }

                        }
                    }
                }
            }

            allTiles[allIndex].IsCollapsed = true;

            if (possibleDirections.Count <= 0)
            {
                allTiles[allIndex].EndTile = true;
                ChooseRandomCollapsedTile();
                return;
            }




            int r = UnityEngine.Random.Range(0, possibleDirections.Count);
            CollapseTile(allTiles[allIndex], possibleDirections[r]);
            SetCollapsedTiles();

            SetCorridor(possibleNeighbors[r]);
        }


        void CollapseTile(MazeTile tile, MazeDirections direction)
        {
            if (direction == MazeDirections.NW)
            {
                foreach (var item in tile.NW)
                {
                    item.SetActive(false);
                }
            }
            else if (direction == MazeDirections.NE)
            {
                foreach (var item in tile.NE)
                {
                    item.SetActive(false);
                }
            }
            else if (direction == MazeDirections.SE)
            {
                foreach (var item in tile.SE)
                {
                    item.SetActive(false);
                }
            }
            else if (direction == MazeDirections.SW)
            {
                foreach (var item in tile.SW)
                {
                    item.SetActive(false);
                }
            }
            tile.IsCollapsed = true;
            tile.HasNeighbor = false;
        }

        public GameObject PlaceObject(Vector3 position, int index, Transform holder)
        {
            position = position / 2;
            position.z = zLevel;
            position += convertedBasePosition;

            int r = index % 2 == 0 ? 0 : 1;
            GameObject go = Instantiate(itemsToPlace[r], holder);
            go.transform.position = position;
            return go;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(gridObject.GetWorldPosition(basePosition.x, basePosition.y), 0.1f);
        }
    } 
}
