using UnityEngine.Networking;
using UnityEngine;

[System.Obsolete]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PalyerSetup : NetworkBehaviour
{

    [SerializeField]
    Behaviour[] componentsToDisable;



    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    // [SerializeField]
    string dontDropLayerName = "DontDrop";

    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPerfab;

    [HideInInspector]
    public GameObject playerUIInstance;



    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();

        }
        else
        {

            //Disable player graphics for local playerGraphics
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDropLayerName));

            // Create player using
            playerUIInstance = Instantiate(playerUIPerfab);
            playerUIInstance.name = playerUIPerfab.name;

            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();

            if (ui == null)
            {
                Debug.LogError("No PlayerUI component on PlayerUI prefab. ");
            }
            ui.SetPlayer(GetComponent<Player>());
            GetComponent<Player>().SetupPlayer();
        }
        
    }

    //[Command]
    //void CmdSetUsername(string playerID, string username)
    //{
    //    Player player = GameManager.GetPlayer(playerID);
    //    if (player != null)
    //    {
    //        Debug.Log(username + " has joined!");
    //        player.username = username;
    //    }
    //}

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();

        GameManager.RegisterPlayer(_netID, _player);

    }


    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);


    }
    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    void OnDisabl()
    {

        Destroy(playerUIInstance);

        GameManager.instance.SetSceneCamerActive(true);

        GameManager.UnRegisterPlayer(transform.name);
    }
}
