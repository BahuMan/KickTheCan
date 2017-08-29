using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

//this script will be added to the playerprefab for bots
public class BotController: NetworkBehaviour
{
    private TagGameController _gameController;
    private PlayerController _player;
    private PlayerIt _playerIt;
    private CharacterController _ctrl;
    private Transform _taggedArea;
    private Vector3 _movement;
    private float nextShot;

    public float ThinkInterval = 5f;
    public float FireInterval = 1;
    public float WalkSpeed = 5f;
    public float ChancesToApproachTagArea = .5f;
    public float SpawnRadius = 30;

    public override void OnStartServer()
    {
        _playerIt = GetComponent<PlayerIt>();
        _player = GetComponent<PlayerController>();
        _ctrl = GetComponent<CharacterController>();
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<TagGameController>();
        _taggedArea = GameObject.FindGameObjectWithTag("TagArea").transform;
        transform.position = _taggedArea.position + Random.onUnitSphere * SpawnRadius + new Vector3(0, SpawnRadius, 0); ;
        StartCoroutine(SetGoal());
    }

    public void Update()
    {
        if (!_ctrl.isGrounded)
        {
            _ctrl.Move(transform.up * Time.deltaTime * -WalkSpeed);
        }
        _ctrl.Move(_movement * Time.deltaTime);
        transform.LookAt(transform.position + _movement);
    }

    private IEnumerator SetGoal()
    {
        while (true)
        {
            yield return new WaitForSeconds(ThinkInterval);
            switch (_player.GetStatus())
            {
                case PlayerController.TagStatus.TAGGED: EvaluateTagged(); break;
                case PlayerController.TagStatus.IT: EvaluateIt(); break;
                case PlayerController.TagStatus.HIDING: EvaluateHiding(); break;
            }
        }
    }

    private void EvaluateTagged()
    {
        ChooseRandomDirection();
    }
    private void EvaluateIt()
    {
        Shooting();
        ChooseRandomDirection();
    }
    private void EvaluateHiding()
    {
        if (Random.Range(0f, 1f) > ChancesToApproachTagArea)
        {
            //50% of time, choose random
            ChooseRandomDirection();
        }
        else
        {
            //50% of time, go straight for tag area
            _movement = _taggedArea.position - transform.position;
            _movement.y = 0f;
            _movement.Normalize();
        }
    }

    private void ChooseRandomDirection()
    {
        Vector2 v = Random.insideUnitCircle.normalized * WalkSpeed;
        _movement = new Vector3(v.x, 0, v.y);
    }

    private void Shooting()
    {
        if (nextShot > Time.time) return; //no shooting yet

        IEnumerable players = _gameController.AllPlayers();
        foreach(PlayerController player in players)
        {
            if (player.gameObject == this.gameObject) continue; //not interesting in shooting myself
            if (player.IsPlayerTagged()) continue; //not interested in players already tagged

            //if not tagged, see if we have an unobstructed view
            RaycastHit info = new RaycastHit();
            if (Physics.SphereCast(transform.position, 1f, player.transform.position - transform.position, out info))
            {
                if (info.transform.gameObject == player.gameObject)
                {
                    //nothing inbetween us and the player -> let's attempt a shot!
                    //turn to the player
                    _movement = (player.transform.position - transform.position);
                    _movement.y = 0;
                    _movement.Normalize();
                    _movement *= WalkSpeed;
                    //also, shoot
                    _playerIt.CmdShoot(transform.position, _movement);
                    break;
                }
                else
                {
                    Debug.Log("Object obstructing clear shot at player " + player.name + ": " + info.transform.name);
                }
            }
            else
            {
                Debug.LogError("This is weird: when looking at the player I don't even see the player? " + player.name);
            }
        }
        _playerIt.CmdShoot(transform.position, transform.forward);
        nextShot = Time.time + this.FireInterval;
    }
}

