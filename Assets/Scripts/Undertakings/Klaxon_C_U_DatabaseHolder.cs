using Klaxon.UndertakingSystem;
using Klaxon.ConversationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Klaxon_C_U_DatabaseHolder : MonoBehaviour
{

    public static Klaxon_C_U_DatabaseHolder instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public UndertakingDatabase undertakingDatabase;
    public ConversationDatabase conversationDatabase;

    private void Start()
    {
        ResetDatabases();
    }

    public void ResetDatabases()
    {
        undertakingDatabase.ResetUndertakings();
        conversationDatabase.ResetConversations();
    }
}


