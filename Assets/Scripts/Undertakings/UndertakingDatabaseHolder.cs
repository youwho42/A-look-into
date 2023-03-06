using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.UndertakingSystem
{
    public class UndertakingDatabaseHolder : MonoBehaviour
    {

        public static UndertakingDatabaseHolder instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        public UndertakingDatabase undertakingDatabase;

        private void Start()
        {
            ResetUndertakings();
        }

        public void ResetUndertakings()
        {
            undertakingDatabase.ResetUndertakings();
        }
    }
}

