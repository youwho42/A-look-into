using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.UndertakingSystem
{
    public class ActivateUndertakingArea : MonoBehaviour
    {
        public UndertakingObject undertaking;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
                undertaking.ActivateUndertaking();
        }
    }
}
