using SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
    public class ButterflySaveSystem : MonoBehaviour, ISaveable
    {
        EntityReproduction reproduction;
        CharacterFlight flight;

        private void Start()
        {
            reproduction = GetComponent<EntityReproduction>();
            flight = GetComponent<CharacterFlight>();
        }

        public object CaptureState()
        {
            return new SaveData
            {
                location = transform.position,
                reproductionTick = reproduction.GetTick(),
                following = flight.followingPlayer
            };
        }

        public void RestoreState(object state)
        {
            StartCoroutine(RestoreStateCo(state));

        }
        public IEnumerator RestoreStateCo(object state)
        {
            var saveData = (SaveData)state;
            yield return new WaitForSeconds(0.3f);
            reproduction.SetTick(saveData.reproductionTick);
            flight.followingPlayer = saveData.following;
            transform.position = saveData.location;
            flight.GetNearestBase();


        }


        [Serializable]
        private struct SaveData
        {
            public SVector3 location;
            public int reproductionTick;
            public bool following;
        }
    }

}