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
    bool gameStarted;


 
    public void StartMiniGame(MiniGameType miniGameType, QI_ItemData item, GameObject gameObject)
    {
        if (!gameStarted)
        {
            foreach (var game in miniGames)
            {
                if (game.miniGameType == miniGameType)
                {
                    game.miniGame.transform.position = PlayerInformation.instance.player.position + new Vector3(1, 0, 100);
                    game.miniGame.SetActive(true);
                    game.miniGame.GetComponentInChildren<IMinigame>().SetupMiniGame(item, gameObject, item.GameDificulty);
                    
                    
                }
            }
            gameStarted = true;
        }
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
        gameStarted = false;
    }
}
