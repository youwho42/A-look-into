using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klaxon.ConversationSystem
{
    [CreateAssetMenu(menuName = "Klaxon/Dialogue Database", fileName = "New_Dialogue_Database")]
    public class ConversationDatabase : ScriptableObject
    {
        
        public List<ConversationObject> conversationObjects= new List<ConversationObject>();

        public void ResetConversations()
        {
            for (int i = 0; i < conversationObjects.Count; i++)
            {
                conversationObjects[i].ResetConversation();
            }
        }

        public void SetAllCoversations()
        {
            conversationObjects = Resources.LoadAll<ConversationObject>("Dialogues/").ToList();
            conversationObjects = conversationObjects.OrderBy(x => x.name).ToList();
        }
    }

}
