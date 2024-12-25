using UnityEngine;


namespace Klaxon.GOAD
{
	public class Set_GOAD_BeliefArea : MonoBehaviour
	{
        public GOAD_ScriptableCondition condition;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (collision.gameObject.transform.position.z == transform.position.z)
                {
                    GOAD_WorldBeliefStates.instance.SetWorldState(condition.Condition, condition.State);
                    GameEventManager.onUndertakingsUpdateEvent.Invoke();
                }
                    
            }
        }
    } 
}
