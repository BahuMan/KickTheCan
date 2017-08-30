using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HUDControl : NetworkBehaviour {

    [SerializeField]
    private Text GameMessage;
    [SerializeField]
    private Text ChatLog;
    [SerializeField]
    private InputField ChatInput;

    [SerializeField]
    private KeyCode ChatHotkey = KeyCode.Return;
    [SerializeField]
    private KeyCode ChatEscape = KeyCode.Escape;

    private bool isChatting = false;

    public delegate void SendMessageEvent(string msg);
    public event SendMessageEvent OnSendMessage;

    private void Reset()
    {
        GameMessage =   GameObject.Find("GameMessage").GetComponent<Text>();
        ChatLog =       GameObject.Find("ChatLog").GetComponent<Text>();
        ChatInput =     GameObject.Find("ChatInput").GetComponent<InputField>();
    }

    private void Update()
    {
        if (isChatting)
        {
            if (Input.GetKeyDown(ChatEscape)) {
                Debug.Log("ESC is considered cancel");
                ChatInput.CancelInvoke();
                ChatInput.DeactivateInputField();
                isChatting = false;
            }
            if (Input.GetKeyDown(KeyCode.Return)) //return is hardcoded for submitting chatline
            {
                Debug.Log("Enter is considered submit");
                isChatting = false;
                ChatInput.DeactivateInputField();
                //this.CmdAddChatLine(ChatInput.text);
                if (OnSendMessage != null) OnSendMessage(ChatInput.text);
            }
        }
        else
        {
            if (Input.GetKeyDown(ChatHotkey))
            {
                ChatInput.ActivateInputField();
                isChatting = true;
            }
        }

    }

    public void AddChatLine(string player, string chatLine)
    {
        //@TODO: sanitize & censorize
        //@TODO: possible duplicate lines when host is also a player?
        ChatLog.text += "<B><color=\"red\">" + player + ": </color></B>" + chatLine + '\n';
    }

    public void OnSubmit(UnityEngine.EventSystems.BaseEventData eventData)
    {
        Debug.Log("OnSubmit");
        isChatting = false;
        ChatInput.DeactivateInputField();
        //this.CmdAddChatLine(ChatInput.text);
        if (OnSendMessage != null) OnSendMessage(ChatInput.text);
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
}
