﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class TagGameController : NetworkBehaviour {

    [SerializeField]
    private float gameStateCheckInterval = 5;

    private List<PlayerController> playerList = new List<PlayerController>(10);
    private PlayerController currentIt = null;

    override public void OnStartServer()
    {
        StartCoroutine(PeriodicalCheck());
    }

    private IEnumerator PeriodicalCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(gameStateCheckInterval);
            CheckGameState();
        }
    }

    [Server]
    private void CheckGameState()
    {
        playerList.RemoveAll(AllDisconnectedPlayers);

        if (playerList.Count == 0)
        {
            Debug.Log("Server couldn't find players. No game state");
            return;
        }

        if (currentIt == null) //happens when "it" quits network game
        {
            StartNewRound(playerList[Random.Range(0, playerList.Count)]);
        }
        else if (currentIt.GetStatus() != PlayerController.TagStatus.IT)
        {
            Debug.LogError("BUG: current IT is " + currentIt.name + " but doesn't have correct status");
            StartNewRound(currentIt);
        }
        else if (playerList.TrueForAll(AllPlayersTaggedOrIT))
        {
            List<PlayerController> tagged = playerList.FindAll(AllPlayersTaggedOrIT);
            StartNewRound(tagged[Random.Range(0, tagged.Count)]);
        }
        else
        {
            //Debug.Log("game is on; no change");
        }
    }

    [Server]
    public void CheatRequestIt(PlayerController it)
    {
        StartNewRound(it);
    }

    [Server]
    private void StartNewRound(PlayerController it)
    {
        //first, change status of previous "it":
        if (currentIt != null && currentIt.gameObject != it.gameObject)
        {
            currentIt.status = PlayerController.TagStatus.HIDING;
        }


        currentIt = it;
        Debug.Log("starting new round - " + it.name + " is IT");
        //@TODO: reset positions?
        foreach (PlayerController pc in playerList)
        {
            if (currentIt == pc)
            {
                pc.status = PlayerController.TagStatus.IT;
            }
            else
            {
                pc.status = PlayerController.TagStatus.HIDING;
            }
        }
        //also, inform TagArea to clear everyone of their "taggged" list
        //i'm doing a search for the tagarea rather than cacheing it
        //so the method will also support changing scenes
        GameObject.FindGameObjectWithTag("TagArea").GetComponent<TaggedAreaControl>().ClearTaggedPlayerList();
    }

    //used to check the generic list:
    private static bool AllPlayersTaggedOrIT(PlayerController pc)
    {
        return pc.GetStatus() == PlayerController.TagStatus.TAGGED || pc.GetStatus() == PlayerController.TagStatus.IT;
    }

    //used to cull the generic list:
    private static bool AllDisconnectedPlayers(PlayerController pc)
    {
        return (pc == null);
    }

    [Server]
    public void RegisterPlayer(PlayerController pc)
    {
        playerList.Add(pc);
        /*
        if (currentIt == null)
        {
            Debug.Log("First player registered is IT");
            currentIt.status = PlayerController.TagStatus.IT;
        }
        else
        {
            currentIt.status = PlayerController.TagStatus.IT;
        }
        */
    }

    public IEnumerable<PlayerController> AllPlayers()
    {
        playerList.RemoveAll(AllDisconnectedPlayers);
        return playerList;
    }

}
