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

    private bool isChatting = false;

    private void Reset()
    {
        GameMessage =   GameObject.Find("GameMessage").GetComponent<Text>();
        ChatLog =       GameObject.Find("ChatLog").GetComponent<Text>();
        ChatInput =     GameObject.Find("ChatInput").GetComponent<InputField>();
    }

    private void Update()
    {
        if (isChatting) return; //don't listen for hotkey

        if (Input.GetKeyDown(ChatHotkey))
        {
            ChatInput.ActivateInputField();
        }
    }

}
