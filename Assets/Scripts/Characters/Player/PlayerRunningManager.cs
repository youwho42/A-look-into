using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerRunningManager : MonoBehaviour
{
    //  Running...after X seconds of running a meter appears that slowly fills, 
    //      and when full you can no longer run until it is back to zero. if you stop running before the meter is filled
    //      it goes back down and you can start running whenever again.but if it actually fills all the way up
    //      you can no longer run at all until it is back to the empty state.

    PlayerInformation player;

    bool runningUIVisible;

    float activateUITime = 3f;
    float UITimer = 0;

    float currentGaugeAmount;
    public float gaugeIncreaseAmount;
    public float gaugeDecreaseAmount;


    bool overMax;
    int overMaxAmount;
    float overTimer;

    public bool shattered = false;
    RunningUI runningUI;

    private void Start()
    {
        player = PlayerInformation.instance;

        
        runningUI = RunningUI.instance;
        GameEventManager.onJumpEvent.AddListener(Jump);
        GameEventManager.onLandEvent.AddListener(Land);
    }
    private void OnDisable()
    {
        GameEventManager.onJumpEvent.RemoveListener(Jump);
        GameEventManager.onLandEvent.RemoveListener(Land);
    }


    void Jump()
    {
        if (shattered || !player.playerController.isGrounded || UIScreenManager.instance.GetCurrentUI() != UIScreenType.None)
            return;

        
        currentGaugeAmount += 0.05f;
        if(currentGaugeAmount >0.1f)
            ActivateUI();
        if (currentGaugeAmount >= 1)
            ShatterGlass();
                
    }
    void Land(int lastZ)
    {
        if (shattered)
            return;

        int z = (int)lastZ - (int)player.currentTilePosition.position.z;
        
        var amount = NumberFunctions.RemapNumber(z, 1.0f, 7.0f, 0.0f, 1.0f);
        amount = Mathf.Clamp01(amount);
        currentGaugeAmount += amount;
        if (currentGaugeAmount > 0.1f)
            ActivateUI();
        if (currentGaugeAmount >= 1)
            ShatterGlass();
    }
    private void Update()
    {
        if (UIScreenManager.instance.inMainMenu)
            return;
        if (runningUIVisible)
        {
            if (player.playerInput.isRunning)
                AugmentGauge();
            else
                DecreaseGauge();


            if (overMax && player.playerInput.isRunning && !shattered)
            {
                overTimer += Time.deltaTime;
                
                if (overTimer > 1f)
                {
                    if (overMaxAmount >= runningUI.cracks.Length)
                    {
                        ShatterGlass();
                        return;
                    }
                    overTimer = 0;
                    runningUI.SetCracks(overMaxAmount);
                    overMaxAmount++;
                 
                }
                
            }
            else
            {
                overMax = false;
            }



            return;
        }
        if (!player.playerInput.isRunning && !runningUIVisible)
            UITimer = 0;

        if (!player.playerInput.isRunning || runningUIVisible)
            return;
        
           

        
        UITimer += Time.deltaTime;

        if (UITimer >= activateUITime)
            ActivateUI();



    }

    private void ShatterGlass()
    {
        GameEventManager.onExhaustedEvent.Invoke(true);
        shattered = true;
        runningUI.SetCracks(-1);
        runningUI.ShatterGlass(true);
        player.playerInput.canRun = false;
        player.playerInput.isRunning = false;
        player.playerInput.runToggle = false;
        overMax = false;
        overMaxAmount = 0;
    }


    void ActivateUI()
    {
        runningUI.ToggleUI(true);
        runningUIVisible = true;
    }

    void DisableUI()
    {
        runningUI.ToggleUI(false);
        UITimer = 0;
        runningUIVisible = false;
        overMaxAmount = 0;
        runningUI.SetCracks(-1);
        overMax = false;
    }


    void AugmentGauge()
    {
        currentGaugeAmount += (gaugeIncreaseAmount * player.statHandler.GetStatCurrentModifiedValue("RunningGauge")) * Time.deltaTime;
        currentGaugeAmount = Mathf.Clamp01(currentGaugeAmount);
        runningUI.SetColor(currentGaugeAmount);
        if (currentGaugeAmount >= 1)
        {
            overMax = true;
        }


    }

    void DecreaseGauge()
    {
        currentGaugeAmount -= gaugeDecreaseAmount * Time.deltaTime;
        currentGaugeAmount = Mathf.Clamp01(currentGaugeAmount);
        runningUI.SetColor(currentGaugeAmount);
        if (currentGaugeAmount <= 0)
        {
            GameEventManager.onExhaustedEvent.Invoke(false);
            shattered = false;
            overMaxAmount = 0;
            player.playerInput.canRun = true;
            DisableUI();
        }
           

    }


    
}
