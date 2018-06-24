using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player_FallHurt : NetworkBehaviour
{
    [SerializeField]
    private float fallHurtRadix = 4.0f; //每秒伤害基数
    [SerializeField]
    private float minHurt = 10; //只有大于这个伤害才计算，否则无视
    [SerializeField]
    private CharacterController characterController;
    
    private Transform myTransform;
    
    
    //起跳高度
    private float takeOffHeight = 0;
    //落地时高度差
    private float flyHeight = 0;
    //是否已经起跳
    public bool isTakeOff = false;

    private int hurt = 0;


    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        myTransform = transform;
    }
    
    void Update()
    {
        if(isLocalPlayer)
        {
            if(characterController.isGrounded)
            {
                if(isTakeOff)
                {
                    flyHeight = takeOffHeight - myTransform.position.y;
                    isTakeOff = false;
                    flyHeight -= 5;
                    hurt = Mathf.FloorToInt(flyHeight * fallHurtRadix);
                    if(hurt > minHurt)
                    {
                        CmdTellServerWhoWasHurt(myTransform.name, hurt);
                    }
                }
            }
            else
            {
                //如果还没有起跳
                if(!isTakeOff)
                {
                    //离地时间
                    takeOffHeight = myTransform.position.y;
                    isTakeOff = true;
                }
            }
        }
    }

    [Command]
    void CmdTellServerWhoWasHurt(string uniqueID, int dmg)
    {
        GameObject go = GameObject.Find(uniqueID);
        go.GetComponent<Player_Health>().DeductHealth(dmg);
    }
}
