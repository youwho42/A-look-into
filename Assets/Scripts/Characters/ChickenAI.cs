using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.GravitySystem;
public class ChickenAI : MonoBehaviour, IAnimal
{

    float timeToStayAtDestination;

    GravityItem gravityItem;
    CanReachTileWalk walk;
    public Animator animator;
    
    public DrawZasYDisplacement home;
    
    static int walking_hash = Animator.StringToHash("IsWalking");
    static int peck_hash = Animator.StringToHash("Peck");

    public float roamingArea;
    
    public float detectionTimeOutAmount = 3;

    bool isPecking;
    bool isSleeping;

    float peckTimer;

    public GameObject nightDisappearObject;

    AnimalSounds sounds;

    bool activeState = true;

    [SerializeField]
    public CurrentState currentState;

    public enum CurrentState
    {
        isWalking,
        isPecking,
        isAtDestination,
        isGoingHome,
        Deviate
    }

    private void Start()
    {
        sounds = GetComponent<AnimalSounds>();
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        walk = GetComponent<CanReachTileWalk>();

        GameEventManager.onTimeHourEvent.AddListener(SetSleepOrWake);
        Invoke("SetSleepState", 1f);
    }

    void SetSleepState()
    {
        SetSleepOrWake(RealTimeDayNightCycle.instance.hours);
    }
    private void OnDestroy()
    {
        GameEventManager.onTimeHourEvent.RemoveListener(SetSleepOrWake);

    }


    private void Update()
    {

        if (!activeState)
            return;

        switch (currentState)
        {
            case CurrentState.isWalking:

                if (sounds.mute)
                    sounds.mute = false;

                if (!isSleeping)
                {
                    walk.Walk();
                    isPecking = false;
                    animator.SetBool(walking_hash, true);
                    if (Vector2.Distance(transform.position, walk.currentDestination) <= 0.01f)
                    {
                        animator.SetBool(walking_hash, false);
                        timeToStayAtDestination = SetRandomRange(2.0f, 12.0f);
                        peckTimer = SetRandomRange(0.2f, 6.0f);
                        currentState = CurrentState.isAtDestination;
                    }
                }
                else
                {
                    walk.SetDestination(home);
                    animator.SetBool(walking_hash, true);
                    currentState = CurrentState.isGoingHome;
                }
                
                break;


            case CurrentState.isPecking:

                animator.SetTrigger(peck_hash);
                animator.SetBool(walking_hash, false);
                if (!AnimatorIsPlaying("NewChickenPeck"))
                {
                    //timeToStayAtDestination = SetRandomRange(2.0f, 6.0f);

                    currentState = CurrentState.isAtDestination;
                }
                
                break;


            case CurrentState.isAtDestination:
                if (!isSleeping)
                {
                    if (sounds.mute)
                        sounds.mute = false;

                    if (!nightDisappearObject.activeSelf)
                        nightDisappearObject.SetActive(true);
                    peckTimer -= Time.deltaTime;
                    if (peckTimer <= 0)
                    {
                        peckTimer = SetRandomRange(0.2f, 5.0f);
                        currentState = CurrentState.isPecking;
                    }


                    timeToStayAtDestination -= Time.deltaTime;
                    if (timeToStayAtDestination <= 0 && !AnimatorIsPlaying("NewChickenPeck"))
                    {
                        walk.SetRandomDestination();
                        animator.SetBool(walking_hash, true);
                        currentState = CurrentState.isWalking;
                    }
                }
                else
                {
                    if (Vector2.Distance(transform.position, home.transform.position) >= 0.01f)
                    {
                        walk.SetDestination(home);
                        animator.SetBool(walking_hash, true);
                        currentState = CurrentState.isGoingHome;
                    }
                    else
                    {
                        nightDisappearObject.SetActive(false);
                    }
                }
                
                break;

            case CurrentState.isGoingHome:
                walk.Walk();
                if (Vector2.Distance(transform.position, home.transform.position) < 0.011f)
                {
                    if (!sounds.mute)
                        sounds.mute = true;
                    timeToStayAtDestination = SetRandomRange(2.0f, 12.0f);
                    animator.SetBool(walking_hash, false);
                    currentState = CurrentState.isAtDestination;
                }
                if(walk.currentDestination != (Vector2)home.transform.position)
                {
                    
                    currentState = CurrentState.Deviate;
                }
                break;

            case CurrentState.Deviate:
                walk.Walk();
                if (Vector2.Distance(transform.position, walk.currentDestination) <= 0.03f)
                {
                    
                    walk.SetDestination(home);
                    animator.SetBool(walking_hash, true);
                    currentState = CurrentState.isGoingHome;
                }
                break;
        }
    }

    float SetRandomRange(float min, float max)
    {
        return Random.Range(min, max);
    }

    

    bool AnimatorIsPlaying(string stateName)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
                return true;
        }
        return false;
    }

    public void SetSleepOrWake(int time)
    {
        if (time >= 19 || time < 6)
        {
            isSleeping = true;
        }
        else if (time >= 6 && time < 19)
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
        
    }

    public void SetActiveState(bool active)
    {
        activeState = active;    
    }
}
