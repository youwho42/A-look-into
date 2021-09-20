using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenAI : MonoBehaviour
{
    public float roamingArea;
    float timeToStayAtDestination;
    bool justTookOff;
    public float detectionTimeOutAmount = 3;
    float detectionTimeOutTimer;

    bool isPecking;
    CharacterWalk walk;
    public Animator animator;
    bool isSleeping;

    float peckTimer;

    [SerializeField]
    public CurrentState currentState;

    public enum CurrentState
    {
        isWalking,
        isPecking,
        isAtDestination
    }

    private void Start()
    {

        DayNightCycle.instance.FullHourEventCallBack.AddListener(SetSleepOrWake);

        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        walk = GetComponent<CharacterWalk>();


    }

    private void Update()
    {

        switch (currentState)
        {
            case CurrentState.isWalking:
                walk.Move();
                isPecking = false;
                animator.SetBool("isWalking", true);
                if (Vector2.Distance(transform.position, walk.currentDestination) <= 0.01f)
                {
                    walk.SetRandomDestination(roamingArea);

                }
                /*flight.Move();
                if (justTookOff)
                {

                    if (currentFlower != null)
                    {
                        currentFlower.tag = "OpenFlower";
                    }

                    detectionTimeOutTimer += Time.deltaTime;
                    if (detectionTimeOutTimer >= detectionTimeOutAmount)
                    {
                        justTookOff = false;
                    }
                }
                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.01f)
                {
                    flight.SetRandomDestination(roamingArea);

                }
                captureCollider.offset = flight.characterSprite.localPosition;*/
                break;


            case CurrentState.isPecking:

                animator.SetTrigger("Peck");
                animator.SetBool("isWalking", false);
                timeToStayAtDestination = SetTimeToStayAtDestination();
                currentState = CurrentState.isAtDestination;
                break;


            case CurrentState.isAtDestination:
                peckTimer -= Time.deltaTime;
                if (peckTimer <= 0)
                {

                    peckTimer = SetRandomRange(0.2f, 2.5f);
                    animator.SetTrigger("Peck");
                }
                timeToStayAtDestination -= Time.deltaTime;
                if (timeToStayAtDestination <= 0)
                {
                    walk.SetRandomDestination(roamingArea);
                    currentState = CurrentState.isWalking;
                    animator.SetBool("isWalking", true);
                }
                /*if (!isSleeping)
                {
                    timeToStayAtDestination -= Time.deltaTime;
                    if (timeToStayAtDestination <= 0)
                    {
                        flight.SetRandomDestination(roamingArea);
                        currentState = CurrentState.isWalking;
                        animator.SetBool("IsLanded", false);
                    }
                }
*/

                break;
        }
    }

    float SetRandomRange(float min, float max)
    {
        return Random.Range(min, max);
    }


    float SetTimeToStayAtDestination()
    {
        return Random.Range(0.5f, 10.0f);
    }

    public void SetSleepOrWake(int time)
    {
        if (time == 20)
        {
            isSleeping = true;
        }
        else if (time == 7)
        {
            isSleeping = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Worm") && !isPecking)
        {
            isPecking = true;
            currentState = CurrentState.isPecking;
        }
        /*if (collision.GetComponent<DrawZasYDisplacement>() != null && !justTookOff && collision.CompareTag("OpenFlower") && currentState == CurrentState.isWalking)
        {
            var temp = collision.GetComponent<DrawZasYDisplacement>();
            flight.SetDestination(temp.transform.position, temp.displacedPosition);
            currentState = CurrentState.isPecking;
            justTookOff = true;
            detectionTimeOutTimer = 0;
            currentFlower = collision;
            currentFlower.tag = "ClosedFlower";
        }
        if (collision.CompareTag("Player"))
        {
            flight.SetRandomDestination(roamingArea * 2);
            currentState = CurrentState.isWalking;
            justTookOff = true;
            detectionTimeOutTimer = 0;
            animator.SetBool("IsLanded", false);
            if (currentFlower != null)
            {
                currentFlower.tag = "ClosedFlower";
            }
        }*/
    }
}
