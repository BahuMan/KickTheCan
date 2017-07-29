using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TaggedAreaControl : NetworkBehaviour
{


    private List<PlayerController> playerList = new List<PlayerController>(10);

    [Server]
    public void AddTaggedPlayer(PlayerController playa)
    {
        playerList.Add(playa);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        if (player.IsPlayerTagged()) return;

        foreach(PlayerController p in playerList)
        {
            if (p != null)
            {
                p.status = PlayerController.TagStatus.TAGGED;
                p.transform.position = this.transform.position;
            }
        }
    }
}
