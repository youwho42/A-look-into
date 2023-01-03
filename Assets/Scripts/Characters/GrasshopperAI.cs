using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.GravitySystem;

public class GrasshopperAI : MonoBehaviour, IAnimal
{
    float timeToStayAtDestination;

    //public InteractAreasManager interactAreas;
    float detectionTimeOutTimer;
    GravityItemNew gravityItem;
    CanReachTileJump jump;
    public Animator animator;
    float idleTimer;
    
    
    static int isGrounded_hash = Animator.StringToHash("IsGrounded");
    static int velocityY_hash = Animator.StringToHash("VelocityY");

    NPC_PlayFootSteps footSteps;
    bool playedFootStep;
    
    float detectionTimeOutAmount;

    bool activeState = true;
    SpriteRenderer animalSprite;
    [SerializeField]
    public PedestrianState currentState;
    public enum PedestrianState
    {
        isJumping,
        isAtDestination
    }

    private void Start()
    {

        gravityItem = GetComponent<GravityItemNew>();

        jump = GetComponent<CanReachTileJump>();

        footSteps = GetComponent<NPC_PlayFootSteps>();

        timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 15.0f));
        animalSprite = gravityItem.itemObject.GetComponent<SpriteRenderer>();
    }

    bool CheckVisibility()
    {
        return animalSprite.isVisible;
    }

    private void Update()
    {
        if (!CheckVisibility())
        {
            if (Time.frameCount % 20 != 0)
                return;
        }
        if (!activeState)
            return;

        if (!gravityItem.isGrounded)
            playedFootStep = false;
        if (gravityItem.isGrounded && !playedFootStep)
        {
            playedFootStep = true;
            footSteps.PlayFootStep();
        }

        switch (currentState)
        {
            case PedestrianState.isJumping:

                jump.Move();

                if (Vector2.Distance(transform.position, jump.currentDestination) <= 0.01f)
                {
                    timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 15.0f));
                    
                    currentState = PedestrianState.isAtDestination;
                }

                break;


            case PedestrianState.isAtDestination:

                timeToStayAtDestination -= Time.deltaTime;

                if (timeToStayAtDestination <= 0)
                {
                    jump.SetRandomDestination();
                    currentState = PedestrianState.isJumping;
                }

                break;
           
        }
    }


    void LateUpdate()
    {
        animator.SetBool(isGrounded_hash, gravityItem.isGrounded);
        animator.SetFloat(velocityY_hash, gravityItem.isGrounded ? 0 : gravityItem.displacedPosition.y);
        
    }
    
    float SetRandomRange(Vector2 minMaxRange)
    {
        return Random.Range(minMaxRange.x, minMaxRange.y);
    }

    float SetRandomRange(float min, float max)
    {
        return Random.Range(min, max);
    }

    public void SetActiveState(bool active)
    {
        activeState = active;
    }
}
