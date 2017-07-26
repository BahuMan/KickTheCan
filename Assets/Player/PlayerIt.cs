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

    private float earliestNewShot = 0f;


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
    void CmdShoot(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(origin, direction, Color.red, 1f);

        bool hitSomething = Physics.Raycast(ray, out hit, shootMaxDistance);
        if (hitSomething)
        {
            //if player was hit, that player is now captured
        }

        RpcProcessShotEffect(hitSomething, hit.point);
    }

    [ClientRpc]
    void RpcProcessShotEffect(bool hitSomething, Vector3 hitLocation)
    {
        //show particle effect, play sound
        Instantiate(this.muzzleFlashPrefab, this.muzzleLocation.position, this.muzzleLocation.rotation);
    }
}
