using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Buildings;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{

    [SerializeField] private GameObject parent;
    [SerializeField] private TMP_Text winnerNameText;

    void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.OnStopClient();
        }
    }

    private void ClientHandleGameOver(string Winner)
    {
        parent.SetActive(true);
        winnerNameText.text = Winner + " has won the game"; 
    }
}
