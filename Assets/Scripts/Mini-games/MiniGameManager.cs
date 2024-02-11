using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    
    public static MiniGameManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    [Serializable]
    public struct MiniGame
    {
        public GameObject miniGame;
        public MiniGameType miniGameType;

    }

    public List<MiniGame> miniGames = new List<MiniGame>();
    public bool gameStarted;


    public void StartMiniGame(MiniGameType miniGameType, QI_ItemData item, GameObject gameObject)
    {
        StartCoroutine(ExecuteMiniGame(miniGameType, item, gameObject));
    }

    public IEnumerator ExecuteMiniGame(MiniGameType miniGameType, QI_ItemData item, GameObject gameObject)
    {
        var screen = LevelManager.instance.HUDBinary == 0 ? UIScreenType.None : UIScreenType.PlayerUI;
        if (!gameStarted && UIScreenManager.instance.CurrentUIScreen() == UIScreenType.None)
        {
            foreach (var game in miniGames)
            {
                if (game.miniGameType == miniGameType)
                {
                    yield return new WaitForSeconds(1.5f);
                    game.miniGame.transform.position = PlayerInformation.instance.player.position + new Vector3(/*1.5f*/ 0, 0, 100);
                    game.miniGame.SetActive(true);
                    game.miniGame.GetComponentInChildren<IMinigame>().SetupMiniGame(item, gameObject, item.GameDificulty);
                    
                }
            }
            UIScreenManager.instance.canChangeUI = false;
            PlayerInformation.instance.TogglePlayerInput(false);
            gameStarted = true;
        }
        yield return null;
    }
    public void EndMiniGame(MiniGameType miniGameType)
    {
        foreach (var game in miniGames)
        {
            if (game.miniGameType == miniGameType)
            {
                game.miniGame.GetComponentInChildren<IMinigame>().ResetMiniGame();
               
            }
        }
        UIScreenManager.instance.canChangeUI = true;
        PlayerInformation.instance.TogglePlayerInput(true);
        PlayerInformation.instance.uiScreenVisible = false;
        gameStarted = false;
    }
}
