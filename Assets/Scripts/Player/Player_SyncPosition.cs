using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class Player_SyncPosition : NetworkBehaviour
{
    //hook指定一个方法，当这个变量在客户端改变的时候会调用它
    [SyncVar(hook = "SyncPostionValues")]
    private Vector3 syncPos;

    [SerializeField]
    Transform myTransform;
    [SerializeField]
    private float lerpRate;
    private float normalLerpRate = 16;
    private float fasterLerpRate = 32;

    private Vector3 lastPos;
    private float threshold = 0.5f;
    
    private List<Vector3> syncPosList = new List<Vector3>();
    [SerializeField]
    private bool useHistoricalLerping = false;
    private float closeEnough = 0.1f;

    void Start()
    {
        lerpRate = normalLerpRate;
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            //如果不是本地玩家的角色，则用差值计算位置
            LerpPosition();
        }
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            //如果是本地玩家角色，把当前位置告诉服务器
            TransmitPosition();
        }
    }

    void LerpPosition()
    {
        if (useHistoricalLerping)
        {
            HistoricalLerping();
        }
        else
        {
            OrdinaryLerping();
        }
    }

    //Command指令，从客户端向服务器发送执行请求，然后由服务器执行它
    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
    }

    //ClientCallback 只会在客户端上调用，而且跟[Client]不同的是，它会忽略警告
    [ClientCallback]
    void TransmitPosition()
    {
        //只有在移动一定距离后才发送这个请求，而不是时时刻刻都发送
        if (Vector3.Distance(myTransform.position, lastPos) > threshold)
        {
            CmdProvidePositionToServer(myTransform.position);
            lastPos = myTransform.position;
        }
    }

    [ClientCallback]
    void SyncPostionValues(Vector3 lastestPos)
    {
        syncPos = lastestPos;
        syncPosList.Add(syncPos);
    }

    void HistoricalLerping()
    {
        if (syncPosList.Count > 0) {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPosList[0], Time.deltaTime * lerpRate);

            if (Vector3.Distance(myTransform.position, syncPosList[0]) < closeEnough) {
                syncPosList.RemoveAt(0);
            }

            if (syncPosList.Count > 10)
            {
                lerpRate = fasterLerpRate;
            }
            else
            {
                lerpRate = normalLerpRate;
            }
        }
    }

    void OrdinaryLerping()
    {
        myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
    }
}
