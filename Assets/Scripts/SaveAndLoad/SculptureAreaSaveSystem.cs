using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SaveSystem
{
	public class SculptureAreaSaveSystem : MonoBehaviour, ISaveable
	{
        public CreateParticleSuperShape superShape;
        public object CaptureState()
        {
            List<bool> active = new List<bool>();
            foreach (var particle in superShape.particleLayers[0].particles)
            {
                active.Add(particle.active);
            }
            return new SaveData
            {
                totalM = superShape.GetTotalM(),
                activeParticles = active,
                isComplete = superShape.sculptureComplete
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            superShape.SetTotalM(saveData.totalM);
            for (int i = 0; i < saveData.activeParticles.Count; i++)
            {
                superShape.particleLayers[0].particles[i].active = saveData.activeParticles[i];
            }
            if(saveData.isComplete)
                superShape.ActivateSculpture();
        }

        [Serializable]
        private struct SaveData
        {
            public int totalM;
            public List<bool> activeParticles;
            public bool isComplete;
        }

    }

}