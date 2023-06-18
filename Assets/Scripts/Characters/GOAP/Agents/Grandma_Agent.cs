using Klaxon.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandma_Agent : GOAP_Agent
{
    public Vector2Int goToHomeTime;
    //public Vector2Int bakeTime;
    public Vector2Int sellTime;

    protected override void Start()
    {
        base.Start();
        
        GOAP_Goal g1 = new GOAP_Goal("IsSleep", 1, false);
        goals.Add(g1, 3);
        
        GOAP_Goal g2 = new GOAP_Goal("IsTalking", 1, false);
        goals.Add(g2, 10);

        GOAP_Goal g4 = new GOAP_Goal("IsWandering", 1, false);
        goals.Add(g4, 1);

        GOAP_Goal g5 = new GOAP_Goal("IsSelling", 1, false);
        goals.Add(g5, 3);
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

        //if (tick >= bakeTime.x && tick < bakeTime.y)
        //    beliefs.SetState("CanBake", 0);
        //else
        //    beliefs.RemoveState("CanBake");

        if (tick >= sellTime.x && tick < sellTime.y)
            beliefs.SetState("CanSell", 0);
        else
            beliefs.RemoveState("CanSell");
    }
}
