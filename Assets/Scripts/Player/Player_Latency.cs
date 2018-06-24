using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class Player_Latency : NetworkBehaviour
{
    private NetworkClient nClient;
    private int latency;
    private Text latencyText;

    public override void OnStartLocalPlayer()
    {
        nClient = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().client;
        latencyText = GameObject.Find("Latency Text").GetComponent<Text>();
    }
    

    void Update()
    {
        if(isLocalPlayer)
        {
            ShowLatency();
        }
    }

    void ShowLatency()
    {
        latency = nClient.GetRTT();
        latencyText.text = latency.ToString();
    }
}
