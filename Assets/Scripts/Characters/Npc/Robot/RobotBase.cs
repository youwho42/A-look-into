using Klaxon.GOAD;
using Klaxon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RobotBase : MonoBehaviour
{
    public GameObject robotSpawn;
     
    public bool hasRobot;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        if(!hasRobot)
        {
            
            var dissolve = DissolveEffect.instance;
            var go = Instantiate(robotSpawn, transform.position, Quaternion.identity);
            var currentRobot = go.GetComponent<GOAD_Scheduler_Robot>();
            currentRobot.homeBase = transform.position;
            currentRobot.SetBeliefState(currentRobot.robotActiveCondition.Condition, false);
            currentRobot.SetBeliefState("AtHome", true);
            var sprites = go.GetComponentsInChildren<SpriteRenderer>();
            foreach (var s in sprites)
            {
                dissolve.StartDissolve(s.material, 2f, true);
            }
            go.GetComponent<SaveableItemEntity>().GenerateId();

            hasRobot = true;
            
        }
    }


}
