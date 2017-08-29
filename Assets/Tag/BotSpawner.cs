using UnityEngine;
using UnityEngine.Networking;

public class BotSpawner: NetworkBehaviour
{
    public int numBots = 4;
    public GameObject PlayerPrefab;

    public override void OnStartServer()
    {
        for (int b=0; b<numBots; ++b)
        {
            GameObject bot = Instantiate(PlayerPrefab);
            bot.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
            bot.AddComponent<BotController>();
            bot.name = "bot " + b;
            NetworkServer.Spawn(bot);
        }
    }
}
