using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Zombie_ID : NetworkBehaviour
{
    [SyncVar]
    public string zombieID;

    private Transform myTransform;



    void Start()
    {
        myTransform = transform;
    }

    void Update()
    {
        if(myTransform.name == "" || myTransform.name == "Zombie(Clone)")
        {
            myTransform.name = zombieID;
        }
    }
}
