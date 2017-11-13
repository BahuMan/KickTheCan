using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class HUDControl : MonoBehaviour {

    [SerializeField]
    private Text GameMessage;
    [SerializeField]
    private ChatView _ChatView;

    private PlayerController localPlayer;
    private bool isChatting;

    private void Reset()
    {
        GameMessage =   GameObject.Find("GameMessage").GetComponent<Text>();
        _ChatView = GameObject.Find("HUD").GetComponent<ChatView>();
    }

    private void Update()
    {
        if (isChatting) return;
        if (localPlayer == null)
        {
            //if there is no local player, there is no need for the GUI/Player interaction
            this.enabled = false;
        }

        if (Input.GetButtonDown("EnableGUI"))
        {
            //localPlayer.EnablePlayer(false);
            localPlayer.GetComponent<FirstPersonController>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
        }
        else if (Input.GetButtonUp("EnableGUI"))
        {
            //localPlayer.EnablePlayer(true);
            localPlayer.GetComponent<FirstPersonController>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void RegisterLocalPlayer(PlayerController p)
    {
        this.localPlayer = p;
        this.enabled = true;

        //bind chat to local player:
        PlayerChat pc = p.GetComponent<PlayerChat>();
        _ChatView.OnSendMessage += pc.OnSendChatMessage;
        //binding in other direction:
        pc.OnReceivedMessageEvent += _ChatView.AddChatLine;
    }

    public void DisplayGameMessage(string msg, float time)
    {
        StartCoroutine(TimedMessage(msg, time));
    }

    private IEnumerator TimedMessage(string msg, float time)
    {
        GameMessage.text = msg;
        yield return new WaitForSeconds(time);
        GameMessage.text = "";
    }

    private void SetChatMode(bool chat)
    {
        this.isChatting = chat;
        localPlayer.EnablePlayer(!chat);
    }
}
