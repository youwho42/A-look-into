using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcIdle : StateMachineBehaviour
{
    protected float timer;
    protected float timeToIdle;
    public Vector2 minMaxTimes;
   
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
        timeToIdle = Random.Range(minMaxTimes.x, minMaxTimes.y);
        
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if(timer >= timeToIdle)
        {
            timer = 0.0f;
            animator.SetTrigger("Walk");
            
        }
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }


}
