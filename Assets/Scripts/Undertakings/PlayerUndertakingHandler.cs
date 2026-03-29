using Klaxon.GOAD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klaxon.UndertakingSystem
{
    public class PlayerUndertakingHandler : MonoBehaviour
    {
        public List<UndertakingObject> activeUndertakings = new List<UndertakingObject>();
        Queue<UndertakingObject> addUndertakingQueue = new Queue<UndertakingObject>();
        [HideInInspector]
        public bool firstUndertakingAquired;

        public GOAD_ScriptableCondition undertakingsActiveCondition;

        public void AddUndertaking(UndertakingObject undertaking)
        {
            
            if (!activeUndertakings.Contains(undertaking))
                addUndertakingQueue.Enqueue(undertaking);
            StartCoroutine(ActivateTasks(undertaking));
            if (!firstUndertakingAquired)
            {
                firstUndertakingAquired = true;
                StartCoroutine(OpenUndertakingTutorial());
            }
        }

        IEnumerator OpenUndertakingTutorial()
        {
            yield return new WaitForSeconds(0.33f);
            GOAD_WorldBeliefStates.instance.SetWorldState(undertakingsActiveCondition);

            AudioManager.instance.PlaySound("Discovery");

            yield return new WaitForSeconds(0.5f);

            GameEventManager.onUndertakingDisplayEvent.Invoke();
        }

        private IEnumerator ActivateTasks(UndertakingObject undertaking)
        {
            yield return new WaitForSeconds(0.1f);
            foreach (var task in undertaking.Tasks)
            {
                task.ActivateTask(undertaking);
            }
            undertaking.TryCompleteQuest();
        }

        public void RestoreUndertaking(UndertakingObject undertaking)
        {
            if (!activeUndertakings.Contains(undertaking))
                activeUndertakings.Add(undertaking);
                
        }
        

        private void Update()
        {
            if (addUndertakingQueue.Count > 0 && UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
                StartCoroutine(CompleteAddUndertaking(addUndertakingQueue.Dequeue()));
        }

        IEnumerator CompleteAddUndertaking(UndertakingObject undertaking)
        {
            
            activeUndertakings.Add(undertaking);
            undertaking.ActivateUndertaking();
            yield return new WaitForSeconds(0.3f);
            AudioManager.instance.StartAquiredAudio();
            GameEventManager.onUndertakingsUpdateEvent.Invoke();
            
        }
        
    }
}

