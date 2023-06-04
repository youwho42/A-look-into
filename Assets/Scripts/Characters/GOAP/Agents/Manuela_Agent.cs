using Klaxon.GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Manuela_Agent : GOAP_Agent
{
    public Vector2Int goToHomeTime;
    
    protected override void Start()
    {
        base.Start();
        GOAP_Goal g1 = new GOAP_Goal("StashSeeds", 1, false);
        goals.Add(g1, 3);
        GOAP_Goal g2 = new GOAP_Goal("IsSleep", 1, false);
        goals.Add(g2, 3);
        GOAP_Goal g3 = new GOAP_Goal("IsWandering", 1, false);
        goals.Add(g3, 1);

        GOAP_Goal g4 = new GOAP_Goal("IsTalking", 1, false);
        goals.Add(g4, 10);
        GameEventManager.onTimeTickEvent.AddListener(SetStateOnTime);
        beliefs.SetState("NotHome", 0);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetStateOnTime);
    }

    private void SetStateOnTime(int tick)
    {
        if (tick >= goToHomeTime.x || tick < goToHomeTime.y)
        {
            beliefs.SetState("IsEvening", 0);
            beliefs.RemoveState("IsDay");
        }
        else
        {
            beliefs.RemoveState("IsEvening");
            beliefs.AddState("IsDay", 0);
        }
            

    }

    

}
