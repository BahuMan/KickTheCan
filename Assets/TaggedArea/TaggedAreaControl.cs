using System.Collections;
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

    private List<PlayerController> playerList = new List<PlayerController>(10);

    [Server]
    public void AddTaggedPlayer(PlayerController playa)
    {
        playerList.Add(playa);
        playa.Teleport(transform.position);
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

        Debug.Log("Server releasing all");
        RpcReleaseAll();
    }

    [ClientRpc]
    public void RpcReleaseAll()
    {
        StartCoroutine(TempUnlock());
    }

    private IEnumerator TempUnlock()
    {
        Debug.Log("Client unlock");
        Lockdown(false);
        yield return new WaitForSeconds(this.UnlockPeriod);
        Debug.Log("Client lock");
        Lockdown(true);
    }

    private void Lockdown(bool locked)
    {
        foreach (GameObject b in barriers)
        {
            b.GetComponent<Renderer>().material = (locked ? this.BlockingMaterial : this.UnblockedMaterial);
            b.GetComponent<Collider>().enabled = locked;
        }
    }
}
