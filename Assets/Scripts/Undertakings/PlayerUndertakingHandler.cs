using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.UndertakingSystem
{
    public class PlayerUndertakingHandler : MonoBehaviour
    {
        public List<UndertakingObject> activeUndertakings = new List<UndertakingObject>();

        public void AddUndertaking(UndertakingObject undertaking)
        {
            if (!activeUndertakings.Contains(undertaking))
            {
                activeUndertakings.Add(undertaking);
                undertaking.ActivateUndertaking();
            }

        }

        public void RestoreUndertaking(UndertakingObject undertaking)
        {
            if (!activeUndertakings.Contains(undertaking))
            {
                activeUndertakings.Add(undertaking);
                
            }

        }


    }
}

