using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class TagGameController : NetworkBehaviour {

    [SerializeField]
    private float gameStateCheckInterval = 5;

    private List<PlayerController> playerList = new List<PlayerController>(10);
    private PlayerController currentIt;

    [ServerCallback]
    private void Start()
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

        if (playerList.Count == 0) return;

        if (currentIt == null)
        {
            Debug.Log("Current IT quit the server; assigning new random IT");
            currentIt = playerList[Random.Range(0, playerList.Count)];
            StartNewRound();
        }
        else if (playerList.TrueForAll(AllPlayersTagged))
        {
            Debug.Log("All players tagged; assigning new IT and restarting round");
            List<PlayerController> tagged = playerList.FindAll(AllPlayersTagged);
            currentIt = tagged[Random.Range(0, tagged.Count)];
            StartNewRound();
        }
    }

    [Server]
    private void StartNewRound()
    {
        //@TODO: reset positions?
        foreach (PlayerController pc in playerList)
        {
            if (currentIt == pc)
            {
                pc.PlayerIsIt();
            }
            else
            {
                pc.PlayerIsHiding();
            }
        }
    }

    //used to check the generic list:
    private static bool AllPlayersTagged(PlayerController pc)
    {
        return pc.IsPlayerTagged();
    }

    //used to cull the generic list:
    private static bool AllDisconnectedPlayers(PlayerController pc)
    {
        return (pc == null);
    }

    public void RegisterPlayer(PlayerController pc)
    {
        playerList.Add(pc);
        if (!currentIt)
        {
            currentIt = pc;
            pc.PlayerIsIt();
        }
        else
        {
            pc.PlayerIsHiding();
        }
    }
}
