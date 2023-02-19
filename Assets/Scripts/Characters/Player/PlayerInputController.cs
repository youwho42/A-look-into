using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerInputController : MonoBehaviour
{
    
    [HideInInspector]
    public Vector2 movement;
    [HideInInspector]
    public bool usingEquippedItem;
    [HideInInspector]
    public bool isRunning;
    [HideInInspector]
    public bool isPaused;
    [HideInInspector]
    public bool isInUI;
    

    InteractWithInteractable interactable;
    float scrollY;

    InputAction move;
    InputAction run;
    InputAction jump;
    InputAction pause;
    InputAction useEquipement;
    InputAction interact;
    InputAction cameraZoom;
    InputAction menuToggle;
    InputAction menuOpen;
    InputAction menuClose;
    InputAction map;
    InputAction stackTransfer;
    InputAction spyglassAim;
    InputAction spyglassChangeSelected;
    InputAction playerMenuBumper;
    InputAction compendiumMenuTrigger;
    PlayerInputActions inputActions;
    
    private void Awake()
    {
        inputActions = new PlayerInputActions();
        
    }
    private void Start()
    {
        interactable = GetComponent<InteractWithInteractable>();
    }

    private void OnEnable()
    {
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
        menuClose.performed += MenuCloseAction;

        map = inputActions.Player.Map;
        map.Enable();
        map.performed += MapAction;

        stackTransfer = inputActions.Player.StackTransfer;
        stackTransfer.Enable();
        stackTransfer.performed += StackTransferOnAction;
        stackTransfer.canceled += StackTransferOffAction;

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
    }
    private void OnDisable()
    {
        move.Disable();
        run.Disable();
        jump.Disable();
        pause.Disable();
        useEquipement.Disable();
        interact.Disable();
        cameraZoom.Disable();
        menuToggle.Disable();
        menuOpen.Disable();
        menuClose.Disable();
        map.Disable();
        spyglassAim.Disable();
        spyglassChangeSelected.Disable();
        playerMenuBumper.Disable();
        compendiumMenuTrigger.Disable();
    }

    void Update()
    {
        
        if (isPaused || isInUI)
        {
            movement = Vector2.zero;
        }
        else
        {
            movement = move.ReadValue<Vector2>();
            movement.y = Mathf.Clamp(movement.y, -0.578125f, 0.578125f);
            movement = movement.normalized;
            if (movement == Vector2.zero)
                isRunning = false;

            if (scrollY != 0)
                GameEventManager.onMouseScrollEvent.Invoke(scrollY);
        }
    }

    private void LateUpdate()
    {
        
        usingEquippedItem = false;
    }

    public void RunAction(InputAction.CallbackContext context) => isRunning = !isRunning;

    public void JumpAction(InputAction.CallbackContext context) => GameEventManager.onJumpEvent.Invoke();

    public void UseEquipementStart(InputAction.CallbackContext context)
    {
        if (!isPaused || !isInUI)
            GameEventManager.onUseEquipmentEvent.Invoke();
        if (MiniGameManager.instance.gameStarted)
            GameEventManager.onMinigameMouseClickEvent.Invoke();
    }

    public void PauseAction(InputAction.CallbackContext context)
    {
        
        if (UIScreenManager.instance.canChangeUI && !LevelManager.instance.isInCutscene)
        {
            if (PlayerInformation.instance.uiScreenVisible)
                return;
            isPaused = !isPaused;
            LevelManager.instance.Pause(isPaused);
        }
    }

    public void InteractAction(InputAction.CallbackContext context)
    {
        interactable.Interact();
    }

    public void MenuToggleAction(InputAction.CallbackContext context) => GameEventManager.onMenuToggleEvent.Invoke();
    public void MenuOpenAction(InputAction.CallbackContext context) => GameEventManager.onMenuDisplayEvent.Invoke();
    public void MenuCloseAction(InputAction.CallbackContext context) => GameEventManager.onMenuHideEvent.Invoke();
    public void MapAction(InputAction.CallbackContext context) => GameEventManager.onMapDisplayEvent.Invoke();
    public void SpyglassAimOnAction(InputAction.CallbackContext context) => GameEventManager.onSpyglassAimEvent.Invoke(true);
    public void SpyglassAimOffAction(InputAction.CallbackContext context) => GameEventManager.onSpyglassAimEvent.Invoke(false);
    public void SpyglassChangeSelected(InputAction.CallbackContext context) => GameEventManager.onSpyglassAimChageSelectedEvent.Invoke((int)spyglassChangeSelected.ReadValue<float>());
    public void StackTransferOnAction(InputAction.CallbackContext context) => GameEventManager.onStackTransferButtonEvent.Invoke(true);
    public void StackTransferOffAction(InputAction.CallbackContext context) => GameEventManager.onStackTransferButtonEvent.Invoke(false);
    public void GamepadBumperAction(InputAction.CallbackContext context) => GameEventManager.onGamepadBumpersButtonEvent.Invoke((int)playerMenuBumper.ReadValue<float>());
    public void GamepadTriggerAction(InputAction.CallbackContext context) => GameEventManager.onGamepadTriggersButtonEvent.Invoke(Mathf.RoundToInt(compendiumMenuTrigger.ReadValue<float>()));

}
