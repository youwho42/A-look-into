using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPG_RobotAI : MonoBehaviour
{
    GravityItem gravityItem;

    MoveToNode moveToNode;
    Vector3Int lastDirection;
    Animator animator;
    public LayerMask obstaclesLayers;
    bool changingDirection;
    
    bool isRetiring;
    bool headOpen;
    bool gathering;
    bool activated;
    bool deviating;

    public bool isActivated = true;
    public Vector3Int homeBaseTilePosition;

    public Transform headBone;

    WorldObjectAudioManager audioManager;

    InteractableSeedRobot interactableContainer;
    public AnimationCurve seedSpawnChance;
    public QI_ItemDatabase seedDatabase;

    float timeToStayIdle;

    // Inventory slot lights system
    public List<RobotLight> inventoryLights = new List<RobotLight>();
    public List<InventoryLightSystem> inventoryLightStates = new List<InventoryLightSystem>();

    [Serializable]
    public struct InventoryLightSystem
    {
        public Color color;
        public float intensity;
        public string state;
    }
    // Robot function lights system
    public RobotLight functionLight;
    public List<StateLightSystem> functionLights = new List<StateLightSystem>();
    RobotStates lightstate;
    [Serializable]
    public struct StateLightSystem
    {
        public Color color;
        public float intensity;
        public RobotStates state;
    }




    public RobotStates currentState;
    public enum RobotStates
    {
        Open,
        Waiting,
        Roaming,
        Deviate,
        Gathering,
        Retiring,
        Deactivated

    }



    private IEnumerator Start()
    {
        interactableContainer = GetComponent<InteractableSeedRobot>();
        gravityItem = GetComponent<GravityItem>();
        moveToNode = GetComponent<MoveToNode>();
        animator = GetComponentInChildren<Animator>();
        audioManager = GetComponent<WorldObjectAudioManager>();
        //GameEventManager.onTimeHourEvent.AddListener(SetActive);

        yield return new WaitForSeconds(0.35f);

        homeBaseTilePosition = gravityItem.surroundingTiles.currentTilePosition;
        timeToStayIdle = UnityEngine.Random.Range(4.0f, 8.0f);
        currentState = RobotStates.Waiting;
        Invoke("SetInventoryLights", 2f);

    }

    /*private void OnDestroy()
    {
        GameEventManager.onTimeHourEvent.RemoveListener(SetActive);

    }*/

    public void SetActive(int time)
    {
        /*if (time == 5 || time == 18)
            currentState = RobotStates.Retiring;*/
    }









    private void Update()
    {
        if (lightstate != currentState)
            SetFunctionLight(currentState);

        if (currentState != RobotStates.Deactivated && !audioManager.IsPlaying("Engine"))
        {
            activated = true;
            audioManager.PlaySound("Deactivate");
            audioManager.PlaySound("Engine");

        }
        else if (currentState == RobotStates.Deactivated && !activated)
            audioManager.StopSound("Engine");




        switch (currentState)
        {
            case RobotStates.Open:

                SetIdleDirection();
                SetInventoryLights();

                if (!interactableContainer.isOpen)
                    currentState = RobotStates.Waiting;

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("RobotOpen") && interactableContainer.isOpen && !headOpen)
                {
                    animator.SetTrigger("Open");
                    headOpen = true;
                    audioManager.PlaySound("OpenHeadAir");
                }

                break;


            case RobotStates.Waiting:


                timeToStayIdle -= Time.deltaTime;
                if (timeToStayIdle <= 0)
                {
                    SetRandomDestination(10);
                   
                    currentState = RobotStates.Roaming;
                    
                }

                // the player is passing by, wait now, either it leaves or it might want to interact... or something...
                SetIdleDirection();
                if (interactableContainer.isOpen)
                    currentState = RobotStates.Open;

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("RobotOpen") && !interactableContainer.isOpen && headOpen)
                {
                    animator.SetTrigger("Close");
                    headOpen = false;
                    audioManager.PlaySound("CloseHeadAir");
                }


                break;


            case RobotStates.Roaming:

                SetMovementDirection();

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MovementTree"))
                    animator.SetTrigger("Wander");

                if (lightstate != currentState)
                    SetFunctionLight(currentState);

                
                moveToNode.Move();

                if (moveToNode.pathComplete)
                {
                    SetRandomDestination(10);

                }


                if (Vector2.Distance(transform.position, moveToNode.currentDestination) <= 0.01f)
                {
                    if (CheckForSeed())
                    {

                        currentState = RobotStates.Gathering;
                    }


                }

                break;


            case RobotStates.Deviate:
                /*if(!deviating && moveToNode.hasPath)
                    SetDeviatePath();
                else if (deviating && moveToNode.hasPath)
                {
                    moveToNode.Move();

                   
                    if (moveToNode.pathComplete)
                    {
                        
                        currentState = RobotStates.Roaming;
                    }
                }*/

                /*SetMovementDirection();
                currentGridLocation.UpdateLocation();

                
                if (moveToNode.pathComplete)
                {
                    Debug.Log("deviated...");
                }*/
                /*if (CheckForGameObject())
                {
                    
                    
                }
                if (Vector2.Distance(transform.position, moveToNode.currentDestination) <= 0.01f)
                {
                    SetRandomDestination();
                    currentState = RobotStates.Roaming;

                }
                
                if (isRetiring)
                {
                    SetHomeDestination();
                    currentState = RobotStates.Retiring;
                }
                else
                {
                    SetRandomDestination();
                    currentState = RobotStates.Roaming;
                }*/

                break;


            case RobotStates.Gathering:

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("RobotGather") && !gathering)
                {
                    audioManager.PlaySound("ArmMove");
                    animator.SetTrigger("Gather");
                    gathering = true;
                }


                if (animator.GetCurrentAnimatorStateInfo(0).IsName("RobotGather") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    interactableContainer.inventory.AddItem(seedDatabase.GetRandomWeightedItem(), 1, false);
                    audioManager.StopSound("ArmMove");
                    SetInventoryLights();
                    SetRandomDestination(10);
                    gathering = false;
                    if (!isRetiring)
                        currentState = RobotStates.Roaming;
                    else
                        currentState = RobotStates.Retiring;

                }

                break;


            case RobotStates.Retiring:
                SetMovementDirection();
                isRetiring = true;
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MovementTree"))
                    animator.SetTrigger("Wander");
                if (lightstate != currentState)
                    SetFunctionLight(currentState);

               
                /*if (CheckForGameObject())
                    currentState = RobotStates.Deviate;*/

                moveToNode.Move();

                if (Vector2.Distance(transform.position, moveToNode.currentDestination) <= 0.01f)
                {
                    if (CheckForSeed())
                    {
                        currentState = RobotStates.Gathering;
                    }
                    SetHomeDestination();
                    if (gravityItem.surroundingTiles.currentTilePosition == homeBaseTilePosition)
                    {
                        isActivated = false;
                        currentState = RobotStates.Deactivated;
                    }
                }
                break;


            case RobotStates.Deactivated:

                SetIdleDirection();

                isRetiring = false;
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("RobotDeactivated"))
                {
                    animator.SetTrigger("Deactivate");
                    audioManager.PlaySound("Deactivate");
                    Invoke("PlayDeactivateSound", .33f);
                }

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("RobotDeactivated") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    activated = false;

                }

                break;
        }


    }
    public void PlayLandedSound()
    {
        audioManager.PlaySound("Landed");
    }
    public void PlayDeactivateSound()
    {
        audioManager.PlaySound("DeactivateSwoosh");
    }
    public void PlayOpenHeadSound()
    {
        audioManager.PlaySound("OpenHeadAir");
    }
    public void PlayCloseHeadSound()
    {
        audioManager.PlaySound("CloseHeadAir");
    }
    void SetMovementDirection()
    {
        if (animator.GetFloat("DirectionX") != GetDirection().x && !changingDirection)
            StartCoroutine(LerpDirection(GetDirection().x));
    }
    void SetIdleDirection()
    {
        if (animator.GetFloat("DirectionX") != 0 && !changingDirection)
            StartCoroutine(LerpDirection(0));
    }



    void SetHomeDestination()
    {
        PathRequestManager.RequestPath(gravityItem.surroundingTiles.currentTilePosition, homeBaseTilePosition, OnPathHomeFound);
    }

    void SetDeviatePath(int distance)
    {
        Vector3Int dest = PathRequestManager.GetRandomDistancedTile(gravityItem.surroundingTiles.currentTilePosition, distance);

        PathRequestManager.RequestPath(gravityItem.surroundingTiles.currentTilePosition, dest, OnDeviatePathFound);
    }
    void OnPathHomeFound(List<Vector3> newPath, bool success)
    {
        if (success)
            moveToNode.PathFound(newPath);
    }

    void SetRandomDestination(int distance)
    {
        Vector3Int dest = PathRequestManager.GetRandomWalkableNode();

        PathRequestManager.RequestPath(gravityItem.surroundingTiles.currentTilePosition, dest, OnPathFound);
    }
    public void OnPathFound(List<Vector3> newPath, bool success)
    {
        if (success)
        {
            moveToNode.PathFound(newPath);
        }

        else
            SetRandomDestination(10);
    }

    public void OnDeviatePathFound(List<Vector3> newPath, bool success)
    {
        if (success)
        {
            deviating = false;
            moveToNode.PathFound(newPath);

        }
        else
        {
            SetDeviatePath(1);
        }
    }

    bool CheckForGameObject()
    {
        var hits = Physics2D.OverlapCircleAll((Vector2)transform.position + GetDirection() / 3, .1f, obstaclesLayers);
        if (hits.Length > 0)
            return true;

        return false;
    }

    bool CheckForSeed()
    {

        var x = seedSpawnChance.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f));

        if (x < 0.1f)
            return true;
        return false;
    }


    Vector2 GetDirection()
    {
        var d = (Vector2)moveToNode.currentDestination - (Vector2)transform.position;
        d = d.normalized;
        return d;
    }


    IEnumerator LerpDirection(float direction)
    {
        changingDirection = true;
        float timeElapsed = 0;
        float lerpDuration = 0.5f;

        float start = animator.GetFloat("DirectionX");
        float end = direction;
        while (timeElapsed < lerpDuration)
        {
            animator.SetFloat("DirectionX", Mathf.Lerp(start, end, timeElapsed / lerpDuration));
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        animator.SetFloat("DirectionX", end);
        changingDirection = false;
        yield return null;
    }


    public void ActivateRobotToggle()
    {
        isActivated = !isActivated;

    }

    void SetFunctionLight(RobotStates state)
    {
        foreach (var light in functionLights)
        {
            if (light.state == state)
            {
                functionLight.SetColor(light.color);
                functionLight.SetIntensity(light.intensity);
                lightstate = light.state;
                return;
            }
        }
    }

    void SetInventoryLights()
    {

        for (int i = 0; i < inventoryLights.Count; i++)
        {
            SetInventoryLightState(inventoryLights[i], "Empty");

        }

        for (int i = 0; i < interactableContainer.inventory.Stacks.Count; i++)
        {
            string lightState = "Full";
            if (interactableContainer.inventory.Stacks[i].Amount < interactableContainer.inventory.Stacks[i].Item.MaxStack)
                lightState = "Holding";



            SetInventoryLightState(inventoryLights[i], lightState);


        }

    }
    void SetInventoryLightState(RobotLight light, string lightState)
    {
        foreach (var state in inventoryLightStates)
        {
            if (state.state == lightState)
            {
                light.SetColor(state.color);
                light.SetIntensity(state.intensity);
            }

        }
    }

    


}

