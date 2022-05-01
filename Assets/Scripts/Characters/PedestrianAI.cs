using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianAI : MonoBehaviour, IAnimal
{

    float timeToStayAtDestination;
    

    float detectionTimeOutTimer;
    GravityItem gravityItem;
    CanReachTileWalk walk;
    public Animator animator;
    float idleTimer;
    bool glide;
    
    public Transform home;
    DrawZasYDisplacement displacmentZ;


    static int walking_hash = Animator.StringToHash("IsWalking");
    static int idle_hash = Animator.StringToHash("Idle");

    [SerializeField]
    public PedestrianState currentState;
    public enum PedestrianState
    {
        isAtDestination,
        isWalking
    }

    private void Start()
    {

        
        
        gravityItem = GetComponent<GravityItem>();
        
        walk = GetComponent<CanReachTileWalk>();
        SetHome(transform);
        idleTimer = SetRandomRange(3, 10);
        timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 15.0f));
        
        displacmentZ = home.GetComponent<DrawZasYDisplacement>();

    }

    void Update()
    {
        switch (currentState)
        {
            case PedestrianState.isAtDestination:
                
                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0)
                {
                    idleTimer = SetRandomRange(0.2f, 5.0f);
                    animator.SetTrigger(idle_hash);
                }

                timeToStayAtDestination -= Time.deltaTime;


                if (timeToStayAtDestination <= 0)
                {
                    walk.SetRandomDestination();
                    
                    animator.SetBool(walking_hash, true);
                    currentState = PedestrianState.isWalking;
                }
                
                

                break;



            case PedestrianState.isWalking:
                walk.Walk();

                if (Vector2.Distance(transform.position, walk.currentDestination) <= 0.01f)
                {
                    timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 15.0f));
                    animator.SetBool(walking_hash, false);
                    currentState = PedestrianState.isAtDestination;

                }
                break;
            
        }
    }

    float SetRandomRange(Vector2 minMaxRange)
    {
        return Random.Range(minMaxRange.x, minMaxRange.y);
    }

    float SetRandomRange(float min, float max)
    {
        return Random.Range(min, max);
    }



    public void SetHome(Transform transform)
    {
        
    }

}
