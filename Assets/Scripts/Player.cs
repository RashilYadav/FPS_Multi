using UnityEngine.Networking;
using UnityEngine;

using System.Collections;


[RequireComponent(typeof(PalyerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {

        get { return _isDead; }
        protected set { _isDead = value; }

    }



    [SerializeField]
    private int maxHealth = 100;



    [SyncVar]
    private int currentHealth;

    [SyncVar]
    public string username = "Loading...";

    public float GetHealthPct()
    {
        return (float)currentHealth / maxHealth;
    }

    public int kills;
    public int deaths;

    [SerializeField]
    private Behaviour[] disableOnDeath;

    [SerializeField]
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;


    public void SetupPlayer()
    {

        if (isLocalPlayer)
        {
            //Switch cameras
            GameManager.instance.SetSceneCamerActive(false);
            GetComponent<PalyerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadCastNewPlayerSetup();
        //wasEnabled = new bool[disableOnDeath.Length];
        //for(int i=0;i<wasEnabled.Length; i++){
        //    wasEnabled[i] = disableOnDeath[i].enabled;
        //}

        //SetDefaults();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }

        SetDefaults();
    }

    //void Update(){
    //    if(!isLocalPlayer){
    //        return;
    //    }
    //    if(Input.GetKeyDown(KeyCode.K)){ 

    //        RpcTakeDamage(999999);
    //    }
    //}

    [ClientRpc]
    public void RpcTakeDamage(int _amount, string _sourceID)
    {

        if (isDead)
        {
            return;
        }

        currentHealth -= _amount;
        Debug.Log(transform.name + " now has " + currentHealth + " health.");
        if (currentHealth <= 0)
        {
            Die(_sourceID);
        }

    }

    private void Die(string _sourceID)
    {
        isDead = true;
        Player sourcePlayer = GameManager.GetPlayer(_sourceID);
        if (sourcePlayer != null)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username);
        }

        deaths++;

        //disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(false);
        }
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;

        }

        GameObject _gxfIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gxfIns, 3f);

        //Switching Camera
        if (isLocalPlayer)
        {

            GameManager.instance.SetSceneCamerActive(true);
            GetComponent<PalyerSetup>().playerUIInstance.SetActive(false);

        }

        // Debug.Log(transform.name + " is DEAD");


        //Call respone method
        StartCoroutine(Respawn());

    }

    [System.Obsolete]
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform _spawnpoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnpoint.position;
        transform.rotation = _spawnpoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();
        Debug.Log("Player Respawn");
    }

    public void SetDefaults()
    {
        isDead = false;
        currentHealth = maxHealth;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }


        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(true);
        }


        if (isLocalPlayer)
        {

            GameManager.instance.SetSceneCamerActive(false);
            GetComponent<PalyerSetup>().playerUIInstance.SetActive(true);

        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = true;

        }

        GameObject _gxfIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gxfIns, 3f);
    }

}
