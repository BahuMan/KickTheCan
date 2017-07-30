using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

[System.Serializable]
public class ToggleEvent: UnityEvent<bool> {}

[RequireComponent(typeof(UnityStandardAssets.Characters.FirstPerson.FirstPersonController))]
public class PlayerController : NetworkBehaviour
{

    [System.Serializable]
    public struct network
    {
        [SerializeField]
        public ToggleEvent onToggleShared;
        [SerializeField]
        public ToggleEvent onToggleLocal;
        [SerializeField]
        public ToggleEvent onToggleRemote;
    }
    [SerializeField]
    network networkOptions;

    [System.Serializable]
    public struct tagStatusChange
    {
        [SerializeField]
        public ToggleEvent onTogglePlayerIt;
        [SerializeField]
        public ToggleEvent onTogglePlayerHiding;
        [SerializeField]
        public ToggleEvent onTogglePlayerTagged;
    }
    [SerializeField]
    tagStatusChange statusChanges;

    public enum TagStatus { IT, TAGGED, HIDING };

    [SyncVar(hook="SetStatus")]
    public TagStatus status;

    private GameObject globalCamera;
    private TagGameController GameCTRL;


    private void Start()
    {
        globalCamera = Camera.main.gameObject;
        EnablePlayer(true);

    }

    public override void OnStartServer()
    {
        GameObject go = GameObject.FindGameObjectWithTag("GameController");
        GameCTRL = go.GetComponent<TagGameController>();
        GameCTRL.RegisterPlayer(this);
    }

    private void EnablePlayer(bool enabled)
    {
        networkOptions.onToggleShared.Invoke(enabled);
        if (isLocalPlayer)
        {
            globalCamera.SetActive(!enabled);
            networkOptions.onToggleLocal.Invoke(enabled);
        }
        else
        {
            networkOptions.onToggleRemote.Invoke(enabled);
        }
    }

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
        Debug.Log("Player IT assigned");
        status = TagStatus.IT;

        statusChanges.onTogglePlayerTagged.Invoke(false);
        statusChanges.onTogglePlayerHiding.Invoke(false);
        //do the activations last, to make sure that no necessary component is turned off again:
        statusChanges.onTogglePlayerIt.Invoke(true);
    }

    private void PlayerIsHiding()
    {
        status = TagStatus.HIDING;

        statusChanges.onTogglePlayerIt.Invoke(false);
        statusChanges.onTogglePlayerTagged.Invoke(false);
        //do the activations last, to make sure that no necessary component is turned off again:
        statusChanges.onTogglePlayerHiding.Invoke(true);
    }

    private void PlayerIsTagged()
    {
        status = TagStatus.TAGGED;

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

    public void Teleport (Vector3 newPosition)
    {
        if (hasAuthority)
        {
            transform.position = newPosition;
        }
    }
}