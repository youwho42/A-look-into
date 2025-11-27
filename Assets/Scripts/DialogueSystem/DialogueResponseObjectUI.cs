using UnityEngine;
using System.Collections.Generic;
using TMPro;


namespace Klaxon.ConversationSystem
{
	public class DialogueResponseObjectUI : MonoBehaviour
	{
		public TextMeshProUGUI currentText;
		int nextIndex;
		DialogueManagerUI dialogueManagerUI;

        private void Start()
        {
			dialogueManagerUI = DialogueManagerUI.instance;
        }
        public void SetResponse(int next, string text)
		{
			nextIndex = next;
			currentText.text = text;
		}

		public void SendResponse()
		{
			dialogueManagerUI.SetNextDialogue(nextIndex);
		}
	} 
}
