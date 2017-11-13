using UnityEngine;
using UnityEngine.UI;

public class ChatView: MonoBehaviour
{

    [SerializeField]
    private Text ChatLog;
    [SerializeField]
    private InputField ChatInput;
    [SerializeField]
    private KeyCode ChatHotkey = KeyCode.Return;
    [SerializeField]
    private KeyCode ChatEscape = KeyCode.Escape;

    //this event will be fired whenever the chatbox has a submitted line of text, to be sent to everyone else in the game
    //the HUDControl will link this event to the local player's OnSendChatMessage()
    public delegate void SendMessageHandler(string msg);
    public event SendMessageHandler OnSendMessage;

    //this event will be fired whenever the chatbox is either enabled or disabled.
    //any component listening for hotkeys should subscribe to this event and disable hotkeys when chat is active
    public delegate void IsChattingHandler(bool isChatting);
    public event IsChattingHandler OnIsChatting;

    private bool isChatting = false;

    private void Reset()
    {
        ChatLog = GameObject.Find("ChatLog").GetComponent<Text>();
        ChatInput = GameObject.Find("ChatInput").GetComponent<InputField>();
    }

    private void Update()
    {
        if (isChatting)
        {
            if (Input.GetKeyDown(ChatEscape))
            {
                Debug.Log("ESC is considered cancel");
                SetChatMode(false);
            }
            if (Input.GetKeyDown(KeyCode.Return)) //return is hardcoded for submitting chatline
            {
                Debug.Log("Enter is considered submit");
                SetChatMode(false);
                if (OnSendMessage != null)
                {
                    Debug.Log("Sending message: " + ChatInput.text);
                    OnSendMessage(ChatInput.text);
                }
            }
        }
        else if (ChatInput.isFocused)
        {
            //ugly hack
            //this can happen when the user unlocks the mouse (using ctrl) and clicks the inputfield
            //somehow, the inputfield got the focus and we're not aware. trigger IsChatting
            SetChatMode(true);
        }
        else if (Input.GetKeyDown(ChatHotkey))
        {
            SetChatMode(true);
        }
    }

    private void SetChatMode(bool chatting)
    {
        Debug.Log("Chatmode = " + chatting + "text = " + ChatInput.text);
        if (chatting)
        {
            ChatInput.ActivateInputField();
        }
        else
        {
            ChatInput.CancelInvoke();
            ChatInput.DeactivateInputField();
        }
        isChatting = chatting;
        if (OnIsChatting != null) OnIsChatting(chatting);
    }

    public void AddChatLine(string player, string chatLine)
    {
        //@TODO: sanitize & censorize
        ChatLog.text += "<B><color=\"red\">" + player + ": </color></B>" + chatLine + '\n';
    }

}
