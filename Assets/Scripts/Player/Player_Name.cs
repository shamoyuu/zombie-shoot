using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player_Name : NetworkBehaviour
{
    [SyncVar]
    public string playerName = "";

    void Start()
    {
        if(isLocalPlayer)
        {
            playerName = GameObject.Find("PlayerManager").GetComponent<PlayerNameManager>().currentName;
            CmdTellServerPlayerName(playerName);
        }
    }

    [Command]
    void CmdTellServerPlayerName(string name)
    {
        playerName = name;
    }
}
