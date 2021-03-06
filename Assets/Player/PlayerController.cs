﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityStandardAssets.CrossPlatformInput;

[System.Serializable]
public class ToggleEvent: UnityEvent<bool> {}

/**
 * This script always remains active on the player, regardless of the status
 * It coordinates the actions (and disables when necessary) the other scripts
 */
[RequireComponent(typeof(UnityStandardAssets.Characters.FirstPerson.FirstPersonController))]
public class PlayerController : NetworkBehaviour
{

    public const int LAYER_DEFAULT = 0;
    public const int LAYER_TAGGED = 9;

    [System.Serializable]
    public struct NetworkOptionsConfig
    {
        [SerializeField]
        public ToggleEvent onToggleShared;
        [SerializeField]
        public ToggleEvent onToggleLocal;
        [SerializeField]
        public ToggleEvent onToggleRemote;
    }
    [SerializeField]
    NetworkOptionsConfig networkOptions;

    [System.Serializable]
    public struct TagStatusChangeConfig
    {
        [SerializeField]
        public ToggleEvent onTogglePlayerIt;
        [SerializeField]
        public ToggleEvent onTogglePlayerHiding;
        [SerializeField]
        public ToggleEvent onTogglePlayerTagged;
    }
    [SerializeField]
    TagStatusChangeConfig statusChanges;

    public enum TagStatus { IT, TAGGED, HIDING, RESET_HIDING, RESET_IT };

    [SyncVar(hook="SetStatus")]
    public TagStatus status;

    private GameObject globalCamera;
    private TagGameController GameCTRL;
    private HUDControl HUD;

    private NetworkAnimator _anim;
    private CharacterController _charCtrl;



    public void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("HUD");
        HUD = go.GetComponent<HUDControl>();
        if (isLocalPlayer)
        {
            _anim = GetComponent<NetworkAnimator>();
            _charCtrl = GetComponent<CharacterController>();
        }

        globalCamera = Camera.main.gameObject;
        EnablePlayer(true);
    }

    private void Update()
    {
        //the 'i' key is a cheat and should be removed from production code

        if (isLocalPlayer)
        {
            if (Input.GetKeyDown("i"))
            {
                Debug.Log("Requesting it");
                CmdRequestIt();
            }

            float speed = CrossPlatformInputManager.GetAxis("Vertical");
            _anim.animator.SetFloat("MoveSpeed", speed);
            if (CrossPlatformInputManager.GetButtonDown("Jump")) _anim.SetTrigger("Jump");
            _anim.animator.SetBool("Grounded", _charCtrl.isGrounded);
        }
    }

    [Command]
    private void CmdRequestIt()
    {
        GameCTRL.CheatRequestIt(this);
    }

    public override void OnStartServer()
    {
        GameObject go = GameObject.FindGameObjectWithTag("GameController");
        GameCTRL = go.GetComponent<TagGameController>();
        GameCTRL.RegisterPlayer(this);
    }

    public void EnablePlayer(bool enabled)
    {
        networkOptions.onToggleShared.Invoke(enabled);
        if (isLocalPlayer)
        {
            HUD.RegisterLocalPlayer(this);
            globalCamera.SetActive(!enabled);
            networkOptions.onToggleRemote.Invoke(false);
            networkOptions.onToggleLocal.Invoke(enabled);
        }
        else
        {
            networkOptions.onToggleLocal.Invoke(false);
            networkOptions.onToggleRemote.Invoke(enabled);
        }

        //since a newly spawned object doesn't trigger syncvar hooks, I'm explicitly setting the status again
        SetStatus(this.status);
    }

    //this method is a network hook for the SyncVar "status"
    private void SetStatus(TagStatus value)
    {
        this.status = value;
        switch (this.status)
        {
            case TagStatus.HIDING: this.PlayerIsHiding(); break;
            case TagStatus.IT: this.PlayerIsIt(); break;
            case TagStatus.TAGGED: this.PlayerIsTagged(); break;
        }
    }

    /**
     * There are three different statuses for a player to be in.
     * Based on any of these statuses, certain components will need to be activated/deactivated
     */
    private void PlayerIsIt()
    {
        if (isLocalPlayer) HUD.DisplayGameMessage("You're IT!", 5f);

        this.gameObject.layer = LAYER_DEFAULT;
        statusChanges.onTogglePlayerTagged.Invoke(false);
        statusChanges.onTogglePlayerHiding.Invoke(false);
        //do the activations last, to make sure that no necessary component is turned off again:
        statusChanges.onTogglePlayerIt.Invoke(true);
    }

    private void PlayerIsHiding()
    {
        if (isLocalPlayer) HUD.DisplayGameMessage("Go HIDE!", 5f);

        this.gameObject.layer = LAYER_DEFAULT;
        statusChanges.onTogglePlayerIt.Invoke(false);
        statusChanges.onTogglePlayerTagged.Invoke(false);
        //do the activations last, to make sure that no necessary component is turned off again:
        statusChanges.onTogglePlayerHiding.Invoke(true);
    }

    private void PlayerIsTagged()
    {
        if (isLocalPlayer) HUD.DisplayGameMessage("You've been tagged ...", 15f);

        this.gameObject.layer = LAYER_TAGGED;
        statusChanges.onTogglePlayerIt.Invoke(false);
        statusChanges.onTogglePlayerHiding.Invoke(false);
        //do the activations last, to make sure that no necessary component is turned off again:
        statusChanges.onTogglePlayerTagged.Invoke(true);
    }

    public bool IsPlayerTagged()
    {
        return status == TagStatus.TAGGED;
    }

    public TagStatus GetStatus()
    {
        return this.status;
    }

    [ClientRpc]
    public void RpcTeleport (Vector3 newPosition)
    {
        if (hasAuthority)
        {
            transform.position = newPosition;
        }
    }


}