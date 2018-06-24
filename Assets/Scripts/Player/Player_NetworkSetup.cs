using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player_NetworkSetup : NetworkBehaviour
{
    [SerializeField]
    Camera playerCam;
    [SerializeField]
    AudioListener audioListener;


    public override void OnStartLocalPlayer()
    {
        GameObject.Find("Scene Camera").SetActive(false);
        //GetComponent<CharacterController>().enabled = true;
        GetComponent<WeaponManager>().enabled = true;
        GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;

        audioListener.enabled = true;
        playerCam.enabled = true;
    }

}
