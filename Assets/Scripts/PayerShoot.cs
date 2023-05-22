using UnityEngine.Networking;
using UnityEngine;

[System.Obsolete]
[RequireComponent(typeof(WeaponManager))]
public class PayerShoot : NetworkBehaviour
{

    private const string PLAYER_TAG = "Player";

    // [SerializeField]
    // private GameObject weaponGFX;


    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;

    private WeaponManager weaponManager;
    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced");
            this.enabled = false;
        }
        weaponManager = GetComponent<WeaponManager>();
        // weaponGFX.layer = LayerMask.NameToLayer(weaponLayerName);
    }
    void Update()
    {

        currentWeapon = weaponManager.GetCurrentWeapon();

        if (currentWeapon.bullets < currentWeapon.maxBullets)
        {
            if (Input.GetButtonDown("Reload"))
            {
            weaponManager.Reload();
            return;
            }
        }

        if (currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }

        }
        else
        {

            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }


        }


    }

    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal){

        RpcDoHitEffect(_pos, _normal);


    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
      GameObject _hitEffect = (GameObject) Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect,2f);
    }


    [Command]
    void CmdOnShoot(){

        RpcDoShootEffect();

    }

    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzelFlash.Play();
    }

    [Client]
    void Shoot()
    {


        // Debug.Log("Test");
        if(!isLocalPlayer && !weaponManager.isReloading){
            return;
        }

        if(currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
            return;
        }

        currentWeapon.bullets--;

        Debug.Log("Remaining bullets: " + currentWeapon.bullets);
        CmdOnShoot();
        

        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
        {
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage, transform.name);
            }

            CmdOnHit(_hit.point, _hit.normal);

        }

        if (currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
        }
    }

    [Command]
    void CmdPlayerShot(string _playerID, int _damage, string _sourceID)
    {

        Debug.Log(_playerID + " has been shot. ");

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage, _sourceID);

        // Destroy(GameObject.Find(_ID));

    }
}
