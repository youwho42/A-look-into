using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Action_SlickAnalyzeSculptures : GOAD_Action
    {
        SlickSculpturesManager sculpturesManager;
        RestoreSculpture currentSculpture;
        public override void StartAction(GOAD_Scheduler_NPC agent)
        {
            base.StartAction(agent);
            sculpturesManager = SlickSculpturesManager.instance;
            currentSculpture = sculpturesManager.GetNextSculpture();
            
        }

        public override void PerformAction(GOAD_Scheduler_NPC agent)
        {
            base.PerformAction(agent);

            success = currentSculpture != null;
            agent.SetActionComplete(true);
        }

        public override void EndAction(GOAD_Scheduler_NPC agent)
        {
            base.EndAction(agent);
            currentSculpture = null;
        }

    }
}
