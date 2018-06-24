using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player_Score : NetworkBehaviour
{
    [SyncVar]
    public int score = 0;

    private Text scoreText;

    private GameObject[] players;

    public override void OnStartLocalPlayer()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        SetScoreText();
        StartCoroutine(SendScoreToServer());
    }

    [Client]
    void SetScoreText()
    {
        if(isLocalPlayer)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            string result = "";
            foreach(GameObject go in players)
            {
                if(go != null)
                {
                    result += (go.GetComponent<Player_Name>().playerName + " : " + go.GetComponent<Player_Score>().score) + "分\n";
                }
            }
            scoreText.text = result;
        }
    }

    [Command]
    void CmdTellServerScore(int sc)
    {
        score = sc;
        RpcSetScoreText();
    }

    [ClientRpc]
    void RpcSetScoreText()
    {
        SetScoreText();
    }

    IEnumerator SendScoreToServer()
    {
        for(;;)
        {
            CmdTellServerScore(score);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
