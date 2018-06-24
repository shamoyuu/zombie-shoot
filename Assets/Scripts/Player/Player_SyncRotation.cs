using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

public class Player_SyncRotation : NetworkBehaviour
{
    //SyncVar 会从服务器同步已经就绪的这个值到客户端
    //hook 指定一个方法，当这个需要同步的变量改变的时候，它会在客户端被调用
    [SyncVar(hook = "OnPlayerRotSynced")]
    float syncPlayerRotation;
    [SyncVar(hook = "OnCameraRotSynced")]
    float syncCameraRotation;

    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    Transform cameraTransform;

    float lerpRate = 20;

    private float lastPlayerRot;
    private float lastCameraRot;
    private float threshold = 3;

    private List<float> syncPlayerRotList = new List<float>();
    private List<float> syncCameraRotList = new List<float>();
    private float closeEnough = 0.3f;

    [SerializeField]
    private bool useHistoricalInterpolation;

    void Start()
    {
        lastPlayerRot = playerTransform.localEulerAngles.y;
        lastCameraRot = cameraTransform.localEulerAngles.x;
    }

    void Update()
    {
        if(!isLocalPlayer)
        {
            //如果不是本地玩家的角色，则用差值计算角度
            LerpRotations();
        }
    }

    void FixedUpdate()
    {
        if(isLocalPlayer)
        {
            //如果是本地玩家角色，把当前位置告诉服务器
            TransmitRotations();
        }
    }

    void LerpRotations()
    {
        if(useHistoricalInterpolation)
        {
            HistoricalInterpolation();
        }
        else
        {
            OrdinaryLerping();
        }
    }

    void HistoricalInterpolation() {
        if(syncPlayerRotList.Count > 0)
        {
            LerpPlayerRotation(syncPlayerRotList[0]);

            if(Mathf.Abs(playerTransform.localEulerAngles.y - syncPlayerRotList[0]) < closeEnough)
            {
                syncPlayerRotList.RemoveAt(0);
            }
        }

        if(syncCameraRotList.Count > 0)
        {
            LerpCameraRotation(syncCameraRotList[0]);

            if(Mathf.Abs(cameraTransform.localEulerAngles.x - syncCameraRotList[0]) < closeEnough)
            {
                syncCameraRotList.RemoveAt(0);
            }
        }
    }

    void OrdinaryLerping()
    {
        LerpPlayerRotation(syncPlayerRotation);
        LerpCameraRotation(syncCameraRotation);
    }

    void LerpPlayerRotation(float rotAngle)
    {
        Vector3 playerNewRot = new Vector3(0, rotAngle, 0);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, Quaternion.Euler(playerNewRot), lerpRate * Time.deltaTime);
    }

    void LerpCameraRotation(float rotAngle)
    {
        Vector3 cameraNewRot = new Vector3(rotAngle, 0, 0);
        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, Quaternion.Euler(cameraNewRot), lerpRate * Time.deltaTime);
    }

    //Command指令，从客户端向服务器发送执行请求，然后由服务器执行它
    [Command]
    void CmdProvideRotationsToServer(float playerRot, float camRot)
    {
        syncPlayerRotation = playerRot;
        syncCameraRotation = camRot;
    }

    //ClientCallback 只会在客户端上调用，而且跟[Client]不同的是，它会忽略警告
    [ClientCallback]
    void TransmitRotations()
    {
        //只有在角度有一定变化后才发送这个请求，而不是时时刻刻都发送
        if(CheckIfBeyongThreshold(playerTransform.localEulerAngles.y, lastPlayerRot) || CheckIfBeyongThreshold(cameraTransform.localEulerAngles.x, lastCameraRot))
        {
            lastPlayerRot = playerTransform.localEulerAngles.y;
            lastCameraRot = cameraTransform.localEulerAngles.x;
            CmdProvideRotationsToServer(lastPlayerRot, lastCameraRot);
        }
    }

    bool CheckIfBeyongThreshold(float rot1, float rot2)
    {
        if(Mathf.Abs(rot1 - rot2) > threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [ClientCallback]
    void OnPlayerRotSynced(float lastestPlayerRot)
    {
        syncPlayerRotation = lastestPlayerRot;
        syncPlayerRotList.Add(syncPlayerRotation);
    }

    [ClientCallback]
    void OnCameraRotSynced(float latestCameraRot)
    {
        syncCameraRotation = latestCameraRot;
        syncCameraRotList.Add(syncCameraRotation);
    }
}
