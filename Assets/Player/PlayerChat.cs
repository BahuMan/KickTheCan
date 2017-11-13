using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerChat : NetworkBehaviour
{

    public delegate void ReceivedMessageHandler(string from, string msg);
    public event ReceivedMessageHandler OnReceivedMessageEvent;

    public void OnSendChatMessage(string msg)
    {
        //this delegate should be called from the chat box, and only if this player was local and registered itself
        CmdBroadcastChatMessage(this.name, msg);
    }

    [Command]
    private void CmdBroadcastChatMessage(string from, string msg)
    {
        RpcBroadcastChatMessage(from, msg);
    }

    [ClientRpc]
    private void RpcBroadcastChatMessage(string from, string msg)
    {
        if (OnReceivedMessageEvent != null) OnReceivedMessageEvent.Invoke(from, msg);
    }
}
