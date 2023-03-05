using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Klaxon.QuestSystem
{
   

    //[CreateAssetMenu(menuName = "Undertakings/Quest")]
    public class QuestObject : ScriptableObject
    {
        public string Name;
        [TextArea]
        public string Description;
        public List<TaskObject> Tasks;
        
    }
}
