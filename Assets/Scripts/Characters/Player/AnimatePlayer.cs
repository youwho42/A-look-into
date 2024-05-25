using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.GravitySystem;
using Klaxon.StatSystem;

public class AnimatePlayer : MonoBehaviour
{

    public Animator animator;
    public bool isIdleSitting;
    GravityItemMovementControllerNew playerMovement;

    public StatChanger gumptionChanger;
    PlayerInputController playerInput;
    bool lostBalance;
    float timeIdle;

    public readonly int balance_hash = Animator.StringToHash("LostBalance");
    public readonly int idleSit_hash = Animator.StringToHash("IdleSit");
    public readonly int isRunning_hash = Animator.StringToHash("IsRunning");
    public readonly int isGrounded_hash = Animator.StringToHash("IsGrounded");
    public readonly int velocityX_hash = Animator.StringToHash("VelocityX");
    public readonly int velocityY_hash = Animator.StringToHash("VelocityY");
    public readonly int pickUp_hash = Animator.StringToHash("PickUp");
    public readonly int isCrafting_hash = Animator.StringToHash("IsCrafting");
    public readonly int isBreathing_hash = Animator.StringToHash("Breathing");
    public readonly int BreathingState_hash = Animator.StringToHash("BreatheState");



    private void Start()
    {
        
        playerMovement = GetComponent<GravityItemMovementControllerNew>();
        playerInput = GetComponent<PlayerInputController>();
        GameEventManager.onTimeTickEvent.AddListener(CheckAddGumption);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(CheckAddGumption);
    }
    void CheckAddGumption(int tick)
    {
        if(isIdleSitting)
            PlayerInformation.instance.statHandler.ChangeStat(gumptionChanger);
    }

    private void Update()
    {
        if (playerMovement.onCliffEdge && !lostBalance && playerMovement.isGrounded)
        {
            playerMovement.isInInteractAction = true;
            lostBalance = true;
            animator.SetTrigger(balance_hash);
            playerMovement.onCliffEdge = false;
            float t = animator.GetCurrentAnimatorStateInfo(0).length;
            Invoke("ResetBalance", t);
        }


        if (GetIdleAnimState())
            timeIdle += Time.deltaTime;
        else
            timeIdle = 0;


        isIdleSitting = timeIdle > 20;
        animator.SetBool(idleSit_hash, isIdleSitting);
          
        
            
    }

    private void LateUpdate()
    {
        
        animator.SetBool(isRunning_hash, playerInput.isRunning);
        animator.SetFloat(velocityX_hash, Mathf.Abs(playerMovement.moveSpeed));
        animator.SetBool(isGrounded_hash, playerMovement.isGrounded);
        if(!playerMovement.isGrounded)
            animator.SetFloat(velocityY_hash, playerMovement.displacedPosition.y);
        else
            animator.SetFloat(velocityY_hash, 0);
    }

    void ResetBalance()
    {
        playerMovement.isInInteractAction = false;
        lostBalance = false;
    }

    public void TriggerPickUp()
    {
        var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (currentClipInfo[0].clip.name != "PickUp")
            animator.SetTrigger(pickUp_hash);
    }

    public void SetCraftAnimation(bool state)
    {
        animator.SetBool(isCrafting_hash, state);
    }

    public void ToggleBreatheAnimation(bool state)
    {
        animator.SetBool(isBreathing_hash, state);
    }
    public void SetBreateState(float t)
    {
        animator.SetFloat(BreathingState_hash, t);
    }

    bool GetIdleAnimState()
    {
        if (PlayerInformation.instance.uiScreenVisible)
            return false;
        var currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (currentClipInfo[0].clip.name == "Idle")
            return true;
        if (currentClipInfo[0].clip.name == "SitOnGround")
            return true;
        return false;
        
    }
}
