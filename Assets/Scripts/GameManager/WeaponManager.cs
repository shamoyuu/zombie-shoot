using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    public GameObject[] guns;
    [SerializeField]
    private Player_Shoot shootScript;

    void Start()
    {
        if(isLocalPlayer)
        {
            shootScript = GetComponent<Player_Shoot>();
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(1);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(2);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeWeapon(3);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeWeapon(4);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeWeapon(5);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            ChangeWeapon(6);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha7))
        {
            ChangeWeapon(7);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            ChangeWeapon(8);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha9))
        {
            ChangeWeapon(9);
        }
    }

    void ChangeWeapon(int index) {
        index--;
        Gun gun = guns[index].GetComponent<Gun>();
        shootScript.audioClipIndex = index;
        shootScript.ChangeGun(gun);
    }
}
