using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

public class WeaponManager : NetworkBehaviour
{
    // Start is called before the first frame update
     [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    private PlayerWeapon currentWeapon;

    private  WeaponGraphics currentGraphics;

    public bool isReloading = false;


    public PlayerWeapon GetCurrentWeapon(){
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics(){
        return currentGraphics;
    }

    void Start()
    {

        EquipWeapon(primaryWeapon);

        
    }

    // Update is called once per frame
    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;   

        GameObject _weaponIns =(GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();
        if(currentGraphics == null)
            Debug.Log("No WeaponGraphics components on the weapon object: " + _weaponIns.name);
        
        if(isLocalPlayer)
            Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer (weaponLayerName));
    }

    public void Reload()
    {
        if(isReloading)
        {
            return;
        }

        StartCoroutine(Reload_Coroutine());
    }

    private IEnumerator Reload_Coroutine ()
    {
        Debug.Log("Reloading...");

        isReloading = true;

        CmdOnReload();

        yield return new WaitForSeconds(currentWeapon.reloadTime);

        currentWeapon.bullets = currentWeapon.maxBullets;

        isReloading = false;
    }

    [Command]
    void CmdOnReload ()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload ()
    {
       Animator anim =  currentGraphics.GetComponent<Animator>();
        if(anim != null)
        {
            anim.SetTrigger("Reload");
        }
    }
}
