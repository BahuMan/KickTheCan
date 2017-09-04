using UnityEngine;
using UnityEngine.Networking;

public class PlayerIt : NetworkBehaviour
{

    [SerializeField]
    private float shotCoolDown = .3f;
    [SerializeField]
    private float shootMaxDistance = 50f;
    [SerializeField]
    private Transform firePosition;
    [SerializeField]
    private GameObject muzzleFlashPrefab;
    [SerializeField]
    private Transform muzzleLocation;
    [SerializeField]
    private LineRenderer shootLine;


    public TaggedAreaControl tagArea;
    private float earliestNewShot = 0f;

    public override void OnStartServer()
    {
        tagArea = GameObject.FindGameObjectWithTag("TagArea").GetComponent<TaggedAreaControl>();
        shootLine.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (earliestNewShot < Time.time && Input.GetButtonDown("Fire1"))
        {
            earliestNewShot = Time.time + shotCoolDown;
            //talk to server
            CmdShoot(firePosition.position, firePosition.forward);
        }
    }

    [Command]
    public void CmdShoot(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(origin, direction, Color.red, 1f);

        bool hitSomething = Physics.Raycast(ray, out hit, shootMaxDistance);
        if (hitSomething)
        {
            PlayerController player = hit.transform.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                //if player was hit, that player is now captured
                player.status = PlayerController.TagStatus.TAGGED;
                tagArea.AddTaggedPlayer(player); //if the "tagged" status hasn't been propagated yet, the TagArea may conclude that everyone has been freed
            }
        }

        RpcProcessShotEffect(hitSomething, hit.point);
    }

    [ClientRpc]
    void RpcProcessShotEffect(bool hitSomething, Vector3 hitLocation)
    {
        //show particle effect, play sound
        Instantiate(this.muzzleFlashPrefab, this.muzzleLocation.position, this.muzzleLocation.rotation);
        if (hitSomething)
        {
            shootLine.SetPosition(1, hitLocation);
        }
        else
        {
            shootLine.SetPosition(1, firePosition.position + firePosition.forward * 20f);
        }
        shootLine.SetPosition(0, muzzleLocation.position);
        shootLine.gameObject.SetActive(true);
        Invoke("DisableShootLine", 1);
    }

    void DisableShootLine()
    {
        shootLine.gameObject.SetActive(false);
    }
}
