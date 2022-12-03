using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAI : MonoBehaviour
{
    public int robotID;

    CurrentGridLocation currentGridLocation;
    MoveToIsometricNode moveToNode;
    Vector3Int lastDirection;
    Animator animator;
    public LayerMask obstaclesLayers;
    bool changingDirection;
    InteractableSeedRobot interactableContainer;
    public AnimationCurve seedSpawnChance;
    public QI_ItemDatabase seedDatabase;
    bool isRetiring;
    bool headOpen;
    bool gathering;
    bool activated;
    bool deviating;
    public bool isActivated = true;
    public Vector3Int homeBaseTilePosition;

    public Transform headBone;

    WorldObjectAudioManager audioManager;


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
        
        currentGridLocation = GetComponent<CurrentGridLocation>();
        moveToNode = GetComponent<MoveToIsometricNode>();
        animator = GetComponent<Animator>();
        interactableContainer = GetComponent<InteractableSeedRobot>();
        audioManager = GetComponent<WorldObjectAudioManager>();
        GameEventManager.onTimeHourEvent.AddListener(SetActive);

        yield return new WaitForSeconds(0.35f);

        currentGridLocation.UpdateLocation();
        currentState = RobotStates.Roaming;
        
        
        Invoke("SetInventoryLights", 2f);
    }

    private void OnDestroy()
    {
        GameEventManager.onTimeHourEvent.RemoveListener(SetActive);

    }

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

                currentGridLocation.UpdateLocation();

                /*if (CheckForGameObject() && !deviating)
                {
                    //currentState = RobotStates.Deviate;
                    deviating = true;
                    SetDeviatePath(1);
                }
*/

                moveToNode.Move();

                if (moveToNode.pathComplete)
                {
                    SetRandomDestination(15);
                    
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
                    SetRandomDestination(15);
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

                currentGridLocation.UpdateLocation();

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
                    if (currentGridLocation.lastTilePosition == homeBaseTilePosition)
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
        PathRequestManager.RequestPath(currentGridLocation.lastTilePosition, homeBaseTilePosition, OnPathHomeFound);
    }

    void SetDeviatePath(int distance)
    {
        Vector3Int dest = PathRequestManager.GetRandomDistancedTile(currentGridLocation.lastTilePosition, distance);

        PathRequestManager.RequestPath(currentGridLocation.lastTilePosition, dest, OnDeviatePathFound);
    }
    void OnPathHomeFound(List<Vector3> newPath, bool success)
    {
        if (success)
            moveToNode.PathFound(newPath);
    }

    void SetRandomDestination(int distance)
    {
        Vector3Int dest = PathRequestManager.GetRandomDistancedTile(homeBaseTilePosition, distance);

        PathRequestManager.RequestPath(currentGridLocation.lastTilePosition, dest, OnPathFound);
    }
    public void OnPathFound(List<Vector3> newPath, bool success)
    {
        if (success)
        {
            
            moveToNode.PathFound(newPath);
        }
            
        else 
            SetRandomDestination(15);
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

        if (x < PlayerInformation.instance.playerStats.GetPlayerLuck(0.1f))
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentState == RobotStates.Gathering || currentState == RobotStates.Deactivated)
            return;

        if (collision.CompareTag("Player"))
        {
            currentState = RobotStates.Waiting;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentState == RobotStates.Gathering || currentState == RobotStates.Deactivated)
            return;

        if (collision.CompareTag("Player"))
        {

            if (isActivated && !isRetiring)
                currentState = RobotStates.Roaming;
            else if (isActivated && isRetiring)
                currentState = RobotStates.Retiring;
            else
                currentState = RobotStates.Deactivated;
        }
    }
}
