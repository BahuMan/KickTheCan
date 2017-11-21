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
    private bool inGUI;

    private void Start()
    {
        if (localPlayer == null)
        {
            //if there is no local player, there is no need for the GUI/Player interaction
            this.enabled = false;
        }
    }

    private void Reset()
    {
        //default for some variables, but this can be overwritten by level desiger:
        GameMessage =   GameObject.Find("GameMessage").GetComponent<Text>();
        _ChatView = GameObject.Find("HUD").GetComponent<ChatView>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("EnableGUI"))
        {
            SetGUIMode(true);
            
        }
        else if (Input.GetButtonUp("EnableGUI"))
        {
            SetGUIMode(false);
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

    private void SetGUIMode(bool gui)
    {
        this.inGUI = gui;
        localPlayer.EnablePlayer(!gui);
        //localPlayer.GetComponent<FirstPersonController>().enabled = !gui;
        Cursor.lockState = gui? CursorLockMode.Locked: CursorLockMode.None;
        Cursor.visible = gui;
    }
}
