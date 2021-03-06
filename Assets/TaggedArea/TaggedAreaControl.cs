﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TaggedAreaControl : NetworkBehaviour
{

    [SerializeField]
    private float UnlockPeriod = 10f;
    [SerializeField]
    private Material BlockingMaterial;
    [SerializeField]
    private Material UnblockedMaterial;
    [SerializeField]
    private List<GameObject> barriers = new List<GameObject>(4);

    private HUDControl _hud;
    private List<PlayerController> playerList = new List<PlayerController>(10);

    public override void OnStartLocalPlayer()
    {
        _hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDControl>();
    }

    [Server]
    public void AddTaggedPlayer(PlayerController playa)
    {
        playerList.Add(playa);
        Debug.Log("adding player " + playa.name + ". Currently " + ListTaggedPlayers()); 
        playa.RpcTeleport(transform.position);
    }

    private string ListTaggedPlayers()
    {
        string sb = playerList.Count + " tagged players: ";
        foreach (var tp in playerList)
        {
            sb += tp.name + ", ";
        }
        return sb;
    }

    [Server]
    //called by the TagGameController when a new round starts
    public void ClearTaggedPlayerList()
    {
        playerList.Clear();
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        if (player.IsPlayerTagged()) return;

        if (playerList.Contains(player))
        {
            Debug.LogError(player.name + " didn't have status TAGGED but was in list: " + ListTaggedPlayers());
            Debug.Break();
            return;
        }

        //change status for everyone currently in the area, and let them go hide again:
        foreach(PlayerController p in playerList)
        {
            if (p != null)
            {
                p.status = PlayerController.TagStatus.HIDING;
            }
        }
        playerList.Clear();
        RpcReleaseAll();
    }

    [ClientRpc]
    public void RpcReleaseAll()
    {
        StartCoroutine(TempUnlock());
        if (isLocalPlayer)
        {
            _hud.DisplayGameMessage("Everyone has been freed, HIDE!", 1f);
        }
    }

    private IEnumerator TempUnlock()
    {
        Lockdown(false);
        yield return new WaitForSeconds(this.UnlockPeriod);
        Lockdown(true);
    }

    private void Lockdown(bool locked)
    {
        foreach (GameObject b in barriers)
        {
            b.GetComponent<Renderer>().material = (locked ? this.BlockingMaterial : this.UnblockedMaterial);
            //colliders are now selective, so they don't need to be fully disabled anymore:
            //b.GetComponent<Collider>().enabled = locked;
        }
    }
}
