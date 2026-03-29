using UnityEngine;
using System.Collections.Generic;

namespace Klaxon.UndertakingSystem
{
    [CreateAssetMenu(menuName = "Undertakings/Task/Reach Location Task")]
    public class Task_ReachLocation : UndertakingTaskObject
    {

        public Texture2D locationAreaTexture;
        [HideInInspector]
        public Color[,] locationArea;

        public override void ActivateTask(UndertakingObject undertakingObject)
        {
            base.ActivateTask(undertakingObject);
            locationArea = NumberFunctions.ConvertImage(locationAreaTexture);
            GameEventManager.onPlayerPositionUpdateEvent.AddListener(CheckLocationReached);
            CheckLocationReached(player.currentTilePosition.position);
        }

        public override void DeactivateTask()
        {
            GameEventManager.onPlayerPositionUpdateEvent.RemoveListener(CheckLocationReached);
        }

        void CheckLocationReached(Vector3Int position)
        {
            var loc = GridManager.instance.GetPlayerIntPosition(position);
            if (locationArea[loc.x, loc.y].a > 0)
                CompleteTask();
        }
    }
}
