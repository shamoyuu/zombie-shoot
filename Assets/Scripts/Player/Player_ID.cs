using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player_ID : NetworkBehaviour
{
    [SyncVar]
    public string playerUniqueIdentity;
    private NetworkInstanceId playerNetID;
    private Transform myTransform;

    public override void OnStartLocalPlayer()
    {
        GetNetIdentity();
        SetIdentity();
    }
    
    void Awake()
    {
        myTransform = transform;
    }

    void Update()
    {
        if(myTransform.name == "" || myTransform.name == "Player(Clone)" || myTransform.name == null)
        {
            SetIdentity();
        }
    }

    [Client]
    void GetNetIdentity()
    {
        playerNetID = GetComponent<NetworkIdentity>().netId;
        CmdTellServerMyIdentity(MakeUniqueIdentity());
    }
    
    [Client]
    void SetIdentity()
    {
        if(!isLocalPlayer)
        {
            myTransform.name = playerUniqueIdentity;
        }
        else
        {
            myTransform.name = MakeUniqueIdentity();
            GameObject.Find("AdminManager").GetComponent<AdminManager>().playerIdentity = myTransform.name;
        }
    }

    [Command]
    void CmdTellServerMyIdentity(string identity)
    {
        playerUniqueIdentity = identity;
    }

    string MakeUniqueIdentity()
    {
        string uniqueIdentity = "Player " + playerNetID;
        return uniqueIdentity;
    }
}
