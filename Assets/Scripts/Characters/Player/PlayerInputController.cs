using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Klaxon.Interactable;
using UnityEngine.InputSystem.Controls;

public class PlayerInputController : MonoBehaviour
{
    
    [HideInInspector]
    public Vector2 movement;
    [HideInInspector]
    public bool usingEquippedItem;
    [HideInInspector]
    public bool isRunning;
    [HideInInspector]
    public bool runToggle;
    [HideInInspector]
    public bool canRun = true;
    [HideInInspector]
    public bool isPaused;
    [HideInInspector]
    public bool isInUI;
    [HideInInspector]
    public Vector2 rightStickPos;
    [HideInInspector]
    public string currentControlScheme = "Keyboard&Mouse";
    
    InteractWithInteractable interactable;
    PlayerActivateSmells playerSmells;
    float scrollY;

    InputAction submit;
    InputAction move;
    InputAction run;
    InputAction jump;
    InputAction pause;
    InputAction useEquipement;
    InputAction interact;
    InputAction longInteract;
    InputAction cameraZoom;
    InputAction menuToggle;
    InputAction menuOpen;
    InputAction menuClose;
    InputAction map;
    InputAction stackTransferGamepad;
    InputAction spyglassAim;
    InputAction spyglassChangeSelected;
    InputAction playerMenuBumper;
    InputAction compendiumMenuTrigger;
    InputAction inventoryRightClickItem;
    InputAction inventoryDragItem;
    InputAction dialogueNext;
    InputAction rotateDecoration;
    InputAction surroundingItems;
    InputAction playerSit;
    InputAction viewStars;

    PlayerInputActions inputActions;
    public PlayerInput playerInput;

    float mouseInactiveTimer;
    Vector2 lastMouseActiveP;

    Vector2Int lastScreenRes;
    private void Awake()
    {
        inputActions = new PlayerInputActions();
        
    }
    private void Start()
    {
        //        bool lockCursor = true;
        //#if UNITY_EDITOR
        //        lockCursor = false;
        //#endif
        //        if (lockCursor)
        //            Cursor.lockState = CursorLockMode.Confined;
        //PlayerSettings.resizableWindow = true;
        interactable = GetComponent<InteractWithInteractable>();
        playerSmells = GetComponent<PlayerActivateSmells>();
        canRun = true;
    }

    private void OnEnable()
    {

        submit = inputActions.UI.Submit;
        submit.Enable();
        submit.performed += SubmitAction;

        move = inputActions.Player.Movement;
        move.Enable();

        run = inputActions.Player.Run;
        run.Enable();
        run.performed += RunAction;

        jump = inputActions.Player.Jump;
        jump.Enable();
        jump.started += JumpAction;
        

        pause = inputActions.Player.Pause;
        pause.Enable();
        pause.performed += PauseAction;

        useEquipement = inputActions.Player.UseEquipement;
        useEquipement.Enable();
        useEquipement.started += UseEquipementStart;

        interact = inputActions.Player.Interact;
        interact.Enable();
        interact.performed += InteractAction;

        longInteract = inputActions.Player.LongInteract;
        longInteract.Enable();
        longInteract.performed += LongInteractAction;
        

        cameraZoom = inputActions.Player.CameraZoom;
        cameraZoom.Enable();
        cameraZoom.performed += x => scrollY = x.ReadValue<float>();

        menuToggle = inputActions.Player.MenuToggle;
        menuToggle.Enable();
        menuToggle.canceled += MenuToggleAction;

        menuOpen = inputActions.Player.MenuOpen;
        menuOpen.Enable();
        menuOpen.performed += MenuOpenAction;

        menuClose = inputActions.Player.MenuClose;
        menuClose.Enable();
        menuClose.performed += CloseUI;

        map = inputActions.Player.Map;
        map.Enable();
        map.performed += MapAction;

        stackTransferGamepad = inputActions.Player.StackTransferGamepad;
        stackTransferGamepad.Enable();
        stackTransferGamepad.canceled += StackTransferAction;

        spyglassAim = inputActions.Player.SpyglassAim;
        spyglassAim.Enable();
        spyglassAim.started += SpyglassAimOnAction;
        spyglassAim.canceled += SpyglassAimOffAction;

        spyglassChangeSelected = inputActions.Player.SpyglassChangeSelected;
        spyglassChangeSelected.Enable();
        spyglassChangeSelected.performed += SpyglassChangeSelected;

        playerMenuBumper = inputActions.Player.PlayerMenu;
        playerMenuBumper.Enable();
        playerMenuBumper.performed += GamepadBumperAction;

        compendiumMenuTrigger = inputActions.Player.CompendiumMenu;
        compendiumMenuTrigger.Enable();
        compendiumMenuTrigger.performed += GamepadTriggerAction;

        inventoryRightClickItem = inputActions.Player.InventoryRightClickItem;
        inventoryRightClickItem.Enable();
        inventoryRightClickItem.started += InventoryRightClickStartAction;
        inventoryRightClickItem.canceled += InventoryRightClickReleaseAction;

        inventoryDragItem = inputActions.Player.InventoryDragItem;
        inventoryDragItem.Enable();
        inventoryDragItem.performed += InventoryDragItemAction;

        dialogueNext = inputActions.Player.DialogueNext;
        dialogueNext.Enable();
        dialogueNext.started += DialogueNextAction;

        rotateDecoration = inputActions.Player.RotateDecoration;
        rotateDecoration.Enable();
        rotateDecoration.started += RotateDecorationAction;

        surroundingItems = inputActions.Player.SurroundingItemsDisplay;
        surroundingItems.Enable();
        surroundingItems.started += DisplaySurroundingItems;

        playerSit = inputActions.Player.Sit;
        playerSit.Enable();
        playerSit.started += PlayerSit;

        viewStars = inputActions.Player.StarMap;
        viewStars.Enable();
        viewStars.started += ViewStars;

    }
    private void OnDisable()
    {
        submit.Disable();
        move.Disable();
        run.Disable();
        jump.Disable();
        pause.Disable();
        useEquipement.Disable();
        interact.Disable();
        longInteract.Disable();
        cameraZoom.Disable();
        menuToggle.Disable();
        menuOpen.Disable();
        menuClose.Disable();
        map.Disable();
        stackTransferGamepad.Disable();
        spyglassAim.Disable();
        spyglassChangeSelected.Disable();
        playerMenuBumper.Disable();
        compendiumMenuTrigger.Disable();
        inventoryRightClickItem.Disable();
        inventoryDragItem.Disable();
        dialogueNext.Disable();
        rotateDecoration.Disable();
        surroundingItems.Disable();
        playerSit.Disable();
        viewStars.Disable();
    }

   

    void Update()
    {
        var lastP = Mouse.current.position.ReadValue();
        if (lastMouseActiveP != lastP)
        {
            lastMouseActiveP = lastP;
            mouseInactiveTimer = 0;
        }
        else
        {
            
            mouseInactiveTimer += Time.unscaledDeltaTime;
        }
            
        Cursor.visible = mouseInactiveTimer < 5;
        if (isPaused || isInUI)
        {
            movement = Vector2.zero;
            isRunning = false;
        }
        else
        {
            movement = move.ReadValue<Vector2>();
            movement.y = Mathf.Clamp(movement.y, -0.578125f, 0.578125f);
            movement = movement.normalized;

            isRunning = runToggle && movement != Vector2.zero && canRun;
            //if (movement == Vector2.zero)
            //{
            //    runningTimer += Time.deltaTime;
            //    if (runningTimer >= stopRunningTime)
            //    {
            //        isRunning = false;
            //        runningTimer = 0;
            //    }
            //}
            //else
            //{
            //    runningTimer = 0;
            //}


            if (scrollY != 0)
                GameEventManager.onMouseScrollEvent.Invoke(scrollY);

            //rightStickPos = inventoryDragItem.ReadValue<Vector2>();
        }
        

        //var stick = rightStickPos;
        if(Gamepad.current != null)
        {
            
            Cursor.visible = playerInput.currentControlScheme == "Gamepad" || mouseInactiveTimer > 5 ? false : true;
           
            var stick = Gamepad.current.rightStick.ReadValue();
            rightStickPos = stick;
            if (stick != Vector2.zero)
            {
                Vector2 currentPosition = Mouse.current.position.ReadValue();
                for (var passedTime = 0f; passedTime < 1; passedTime += Time.unscaledDeltaTime)
                {
                    currentPosition += stick * 15 * Time.unscaledDeltaTime;
                }
                Mouse.current.WarpCursorPosition(currentPosition);
            }
        }


        


        if (currentControlScheme != playerInput.currentControlScheme)
        {
            
            currentControlScheme = playerInput.currentControlScheme;
            GameEventManager.onControlSchemeChangedEvent.Invoke(currentControlScheme);
        }

        
            
    }

    private void LateUpdate()
    {
        
        usingEquippedItem = false;
    }

    //public void RunAction(InputAction.CallbackContext context) => isRunning = !isRunning;

    public void RunAction(InputAction.CallbackContext context) 
    { 
        if(canRun && UIScreenManager.instance.GetIsCurrentUI(UIScreenType.None))
            runToggle = !runToggle; 
    }
        
    

    public void JumpAction(InputAction.CallbackContext context) => GameEventManager.onJumpEvent.Invoke();

    public void UseEquipementStart(InputAction.CallbackContext context)
    {
        if (LevelManager.instance.inPauseMenu)
            return;
        if (!isPaused || !isInUI)
            GameEventManager.onUseEquipmentEvent.Invoke();
        if (MiniGameManager.instance.gameStarted)
            GameEventManager.onMinigameMouseClickEvent.Invoke();
        if(isInUI && UIScreenManager.instance.GetMapOpen())
            GameEventManager.onMapClickEvent.Invoke();
    }

    public void PauseAction(InputAction.CallbackContext context)
    {
        
        
        if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
        {
            isPaused = true;
            UIScreenManager.instance.SetPauseScreen(isPaused);
            
            //LevelManager.instance.Pause(isPaused);
        }
        else
        {
            
            CloseUI(context);
        }
            
    }

    public void CloseUI(InputAction.CallbackContext context)
    {
        if (!UIScreenManager.instance.inMainMenu)
        {
            isPaused = false;
            GameEventManager.onMenuHideEvent.Invoke();
        }
    }

    public void InteractAction(InputAction.CallbackContext context)
    {
        interactable.Interact();
    }
    public void LongInteractAction(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
            interactable.LongInteract();
    }
    


    public void SubmitAction(InputAction.CallbackContext context) => GameEventManager.onSubmitEvent.Invoke();


    public void MenuToggleAction(InputAction.CallbackContext context) => GameEventManager.onMenuToggleEvent.Invoke();
    public void MenuOpenAction(InputAction.CallbackContext context) => GameEventManager.onMenuDisplayEvent.Invoke();
    //public void MenuCloseAction(InputAction.CallbackContext context) => GameEventManager.onMenuHideEvent.Invoke();

    public void MapAction(InputAction.CallbackContext context) => GameEventManager.onMapDisplayEvent.Invoke();

    public void SpyglassAimOnAction(InputAction.CallbackContext context) => GameEventManager.onSpyglassAimEvent.Invoke(true);
    public void SpyglassAimOffAction(InputAction.CallbackContext context) => GameEventManager.onSpyglassAimEvent.Invoke(false);
    public void SpyglassChangeSelected(InputAction.CallbackContext context) => GameEventManager.onSpyglassAimChageSelectedEvent.Invoke((int)spyglassChangeSelected.ReadValue<float>());
    
    public void StackTransferAction(InputAction.CallbackContext context) => GameEventManager.onStackTransferGamepadEvent.Invoke();
    
    public void GamepadBumperAction(InputAction.CallbackContext context) => GameEventManager.onGamepadBumpersButtonEvent.Invoke((int)playerMenuBumper.ReadValue<float>());
    public void GamepadTriggerAction(InputAction.CallbackContext context) => GameEventManager.onGamepadTriggersButtonEvent.Invoke(Mathf.RoundToInt(compendiumMenuTrigger.ReadValue<float>()));

    public void InventoryDragItemAction(InputAction.CallbackContext context) => GameEventManager.onInventoryDragEvent.Invoke();
    public void InventoryRightClickStartAction(InputAction.CallbackContext context) => GameEventManager.onInventoryRightClickEvent.Invoke();
    public void InventoryRightClickReleaseAction(InputAction.CallbackContext context) => GameEventManager.onInventoryRightClickReleaseEvent.Invoke();

    public void DialogueNextAction(InputAction.CallbackContext context) => GameEventManager.onDialogueNextEvent.Invoke();
    public void RotateDecorationAction(InputAction.CallbackContext context) => GameEventManager.onRotateDecoration.Invoke();
    public void DisplaySurroundingItems(InputAction.CallbackContext context) => GameEventManager.onSurroundingItemsEvent.Invoke();
    public void PlayerSit(InputAction.CallbackContext context) => GameEventManager.onSitEvent.Invoke();
    public void ViewStars(InputAction.CallbackContext context) => GameEventManager.onViewStarsEvent.Invoke();

}
