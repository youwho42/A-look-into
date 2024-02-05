using Klaxon.MazeTech;
using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class SaveMazeSystem : MonoBehaviour, ISaveable
    {

        public MazeCreator mazeCreator;

        public MazeStringGame stringGame;
        
        public object CaptureState()
        {
            // string positions
            List<SVector3> stringPositions = new List<SVector3>();
            for (int i = 0; i < stringGame.line.positionCount; i++)
            {
                stringPositions.Add(stringGame.line.GetPosition(i));
            }

            // Post Positions
            List<SVector3> posts = new List<SVector3>();
            for (int i = 0; i < mazeCreator.endPostItems.Count; i++)
            {
                posts.Add(mazeCreator.endPostItems[i].transform.position);
            }

            // Hedges
            bool[,] hedges = new bool[mazeCreator.mazeSize, mazeCreator.mazeSize];
            for (int x = 0; x < mazeCreator.mazeSize; x++)
            {
                for (int y = 0; y < mazeCreator.mazeSize; y++)
                {
                    hedges[x, y] = false;
                    if(mazeCreator.hedgePieces[x, y] != null)
                        hedges[x, y] = mazeCreator.hedgePieces[x, y].activeSelf;
                }
            }

            return new SaveData
            {
                hedgePieces = hedges,
                postPositions = posts,
                mazeState = mazeCreator.mazeSet,

                index = stringGame.currentIndex,
                mazeComplete = stringGame.mazeComplete,
                linePositions = stringPositions,

                doorIsOpen = mazeCreator.mazeDoor.isOpen
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;

            // maze
            List<Vector3> posts = new List<Vector3>();
            for (int i = 0; i < mazeCreator.endPostItems.Count; i++)
            {
                posts.Add(saveData.postPositions[i]);
            }
            mazeCreator.SetMazeFromSave(saveData.hedgePieces, posts, saveData.mazeState);

            // string
            Vector3[] stringPos = new Vector3[saveData.linePositions.Count];
            for (int i = 0; i < saveData.linePositions.Count; i++)
            {
                stringPos[i] = saveData.linePositions[i];
            }
            stringGame.SetStringGameFromSave(saveData.index, saveData.mazeComplete, stringPos);

            // door
            mazeCreator.mazeDoor.SetDoorFromSave(saveData.doorIsOpen);
        }

        [Serializable]
        private struct SaveData
        {
            // maze
            public bool[,] hedgePieces;
            public List<SVector3> postPositions;
            public bool mazeState;

            // string
            public int index;
            public bool mazeComplete;
            public List<SVector3> linePositions;

            // door
            public bool doorIsOpen;
        }
    } 
}

