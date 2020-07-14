using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class NpcWalkToDestination : StateMachineBehaviour
{
    GetDestination getDestination;
    Vector3 destination;
    Vector3 direction;
    Transform transform;
    NpcMovement npcMovement;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        npcMovement = animator.GetComponent<NpcMovement>();
        transform = animator.GetComponent<Transform>();
        getDestination = animator.GetComponent<GetDestination>();
        destination = getDestination.SetDestination();
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        direction = destination - transform.position;
        npcMovement.direction = direction.normalized;
        float dist = Vector2.Distance(transform.position, destination);
        if(dist <= .1f)
        {
            animator.SetTrigger("Idle");
        }

        if (animator.GetComponentInParent<Rigidbody2D>().velocity != Vector2.zero)
        {
            return;
        }
        getDestination.SetDestination();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        npcMovement.direction = Vector3.zero;

    }

    
    
    

}
