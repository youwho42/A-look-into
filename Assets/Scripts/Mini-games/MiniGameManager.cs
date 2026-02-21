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

    public void StartMiniGame(MiniGameType miniGameType, PokableItem pokable)
    {
        StartCoroutine(ExecuteMiniGame(miniGameType, pokable));
    }

    public void StartMiniGame(MiniGameType miniGameType, QI_ItemData item, GameObject gameObject)
    {
        StartCoroutine(ExecuteMiniGame(miniGameType, item, gameObject));
    }
    public void StartMiniGame(MiniGameType miniGameType, SpadeJunkPileInteractor junkPile)
    {
        StartCoroutine(ExecuteMiniGame(miniGameType, junkPile));
    }
    public void StartMiniGame(MiniGameType miniGameType, SpadeInteractable spadeInteractable)
    {
        StartCoroutine(ExecuteMiniGame(miniGameType, spadeInteractable));
    }

    public IEnumerator ExecuteMiniGame(MiniGameType miniGameType, QI_ItemData item, GameObject gameObject)
    {
        
        if (!gameStarted && UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
        {
            foreach (var game in miniGames)
            {
                if (game.miniGameType == miniGameType)
                {
                    UIScreenManager.instance.SetMiniGameUI(true);
                    yield return new WaitForSeconds(1.0f);
                    if (gameObject.TryGetComponent(out TreeRustling tree))
                        tree.Affect(true);
                    yield return new WaitForSeconds(0.5f);
                    game.miniGame.transform.position = PlayerInformation.instance.player.position + new Vector3(0, 0, 100);
                    game.miniGame.SetActive(true);
                    game.miniGame.GetComponentInChildren<IMinigame>().SetupMiniGame(item, gameObject, item.GameDificulty);
                    break;
                }
            }
            
            
            gameStarted = true;
        }
        yield return null;
    }

    public IEnumerator ExecuteMiniGame(MiniGameType miniGameType, PokableItem pokable)
    {

        if (!gameStarted && UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
        {
            foreach (var game in miniGames)
            {
                if (game.miniGameType == miniGameType)
                {
                    UIScreenManager.instance.SetMiniGameUI(true);
                    yield return new WaitForSeconds(1.5f);
                    game.miniGame.transform.position = PlayerInformation.instance.player.position + new Vector3(0, 0, 100);
                    game.miniGame.SetActive(true);
                    game.miniGame.GetComponentInChildren<IMinigame>().SetupMiniGame(pokable, pokable.GameDificulty);
                    break;
                }
            }


            gameStarted = true;
        }
        yield return null;
    }

    public IEnumerator ExecuteMiniGame(MiniGameType miniGameType, SpadeJunkPileInteractor junkPile)
    {

        if (!gameStarted && UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
        {
            foreach (var game in miniGames)
            {
                if (game.miniGameType == miniGameType)
                {
                    UIScreenManager.instance.SetMiniGameUI(true);
                    yield return new WaitForSeconds(1.5f);
                    game.miniGame.transform.position = PlayerInformation.instance.player.position + new Vector3(0, 0, 100);
                    game.miniGame.SetActive(true);
                    game.miniGame.GetComponentInChildren<IMinigame>().SetupMiniGame(junkPile, junkPile.gameDificulty);
                    break;
                }
            }


            gameStarted = true;
        }
        yield return null;
    }

    public IEnumerator ExecuteMiniGame(MiniGameType miniGameType, SpadeInteractable spadeInteractable)
    {

        if (!gameStarted && UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
        {
            foreach (var game in miniGames)
            {
                if (game.miniGameType == miniGameType)
                {
                    UIScreenManager.instance.SetMiniGameUI(true);
                    yield return new WaitForSeconds(1.5f);
                    game.miniGame.transform.position = PlayerInformation.instance.player.position + new Vector3(0, 0, 100);
                    game.miniGame.SetActive(true);
                    game.miniGame.GetComponentInChildren<IMinigame>().SetupMiniGame(spadeInteractable, spadeInteractable.gameDificulty);
                    break;
                }
            }


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
                game.miniGame.SetActive(false);
            }
        }
        UIScreenManager.instance.SetMiniGameUI(false);
        gameStarted = false;
    }
}
